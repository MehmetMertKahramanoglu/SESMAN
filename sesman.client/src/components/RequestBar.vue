<script setup lang="ts">
import { ref } from 'vue';
import { useRequestStore } from '@/Stores/requestStore';
import { useCollectionStore } from '@/Stores/collectionStore';

// Store kısımlarına erişmek için
const store = useRequestStore();
const collectionStore = useCollectionStore();

// Dropdown kısmında gösterilecek HTTP metod seçenekleri
const httpMethods = ['GET', 'POST', 'PUT', 'PATCH', 'DELETE'];

const isSaveModalOpen = ref(false);
const newRequestName = ref('');
const selectedCollectionId = ref('');

// Backende gönderirken string'leri int'e çevirme kısmı
const methodMap: Record<string, number> = {
  'GET': 1, 'POST': 2, 'PUT': 3,'PATCH': 4, 'DELETE': 5 
};

const openSaveModal = () => {
  isSaveModalOpen.value = true;
};

const closeSaveModal = () => {
  isSaveModalOpen.value = false;
  newRequestName.value = '';
  selectedCollectionId.value = '';
};

// Kaydet butonuna basıldığında çalışacak fonksiyon
const confirmSave = async () => {
  if (!newRequestName.value || !selectedCollectionId.value) {
    alert("Please enter a name and select a folder!");
    return;
  }

 let finalBody = '';
  
  if (store.bodyType === 'none') {
    finalBody = '';
  } else if (store.bodyType === 'binary') {
    finalBody = store.binaryContent; // Base64 dosyası
  } else if (store.bodyType === 'form-data') {
    finalBody = JSON.stringify(store.formDataList); // Form listesi 
  } else if (store.bodyType === 'x-www-form-urlencoded') {
    finalBody = JSON.stringify(store.urlEncodedList);
  } else if (store.bodyType === 'GraphQL') {
    try {
      finalBody = JSON.stringify({
        query: store.graphqlQuery,
        variables: store.graphqlVariables ? JSON.parse(store.graphqlVariables) : {}
      });
    } catch(e) {
      // Eğer kullanıcı variables kısmına geçersiz bir JSON yazarsa çökmesin diye
      finalBody = JSON.stringify({ query: store.graphqlQuery, variables: {} });
    }
  } else if (store.bodyType === 'raw') {
    finalBody = store.body;
  }

  // RequestStore'daki verileri toplayıp backend'in beklediği formata çevirdim.
  const payload = {
    name: newRequestName.value,
    collectionId: selectedCollectionId.value,
    url: store.url,
    method: methodMap[store.method.toUpperCase()] || 0,
    
  
    body: finalBody,
    bodyType: store.bodyType, 
    
    // Header ve Parametreleri ayarlıyoruz
    savedRequestHeaders: store.requestHeaders.map(h => ({ key: h.key, value: h.value })),
    savedRequestParameters: store.requestParameters.map(p => ({ key: p.key, value: p.value }))
  };

  // Veriyi Collection Store'a gönder ve backend'e kaydet
  const isSuccess = await collectionStore.saveNewRequest(payload);
  
  if (isSuccess) {
    alert("The template was successfully saved to the folder!");
    closeSaveModal(); // Başarılıysa pencereyi kapat
  }
};
</script>

<template>
  <div class="request-bar">
    <!-- HTTP metodu seçimi -->
    <select v-model="store.method" class="modern-select">
      <option v-for="method in httpMethods" :key="method" :value="method">
        {{ method }}
      </option>
    </select>

    <!-- İstek gönderilecek URL -->
    <input 
      v-model="store.url" 
      type="text" 
      class="modern-input" 
      placeholder="https://api.example.com/endpoint"
      @keyup.enter="store.sendRequest()"
    />

    <!-- İsteği gönderen buton -->
    <button class="modern-btn primary" @click="store.sendRequest()" :disabled="store.isLoading">
      {{ store.isLoading ? 'Bekleniyor...' : 'Send' }}
    </button>

    <!-- Kaydet butonu -->
    <button class="modern-btn outline" @click="openSaveModal">
      Save
    </button>
  </div>


  <Transition name="modal-fade">
    <!-- @click.self sayesinde arkaplana tıklayınca modal kapanır -->
    <div v-if="isSaveModalOpen" class="modal-overlay" @click.self="closeSaveModal">
      <div class="modal-content">
        
        <!-- Modal Başlık Alanı -->
        <div class="modal-header">
          <h3>Save The Request</h3>
          <button class="close-icon" @click="closeSaveModal">&times;</button>
        </div>
        
        <!-- Modal İçerik -->
        <div class="modal-body">
          <div class="form-group">
            <label>Request Name</label>
            <input 
              v-model="newRequestName" 
              type="text" 
              class="modern-input w-full"
              placeholder="Example: Get User Profile" 
            />
          </div>

          <div class="form-group">
            <label>Target Folder</label>
            <select v-model="selectedCollectionId" class="modern-select w-full">
              <option disabled value="">Select a folder...</option>
              <option v-for="folder in collectionStore.collections" :key="folder.id" :value="folder.id">
                📁 {{ folder.name }}
              </option>
            </select>
          </div>
        </div>

        <!-- Modal Aksiyon Butonları -->
        <div class="modal-footer">
          <button class="modern-btn text-btn" @click="closeSaveModal">
            Cancel
          </button>
          <button class="modern-btn primary shadow" @click="confirmSave">
            Save
          </button>
        </div>

      </div>
    </div>
  </Transition>
</template>

<style scoped>


/* GENEL DÜZEN */
.request-bar {
  display: flex;
  gap: 12px;
  align-items: center;
}

/* MODERN INPUT & SELECT (Ortak Stil) */
.modern-input, .modern-select {
  padding: 10px 14px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 14px;
  color: #334155;
  background-color: #f8fafc;
  outline: none;
  transition: all 0.2s ease-in-out;
}

.modern-input:focus, .modern-select:focus {
  background-color: #ffffff;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15); /* Mavi focus hare efekti */
}

.modern-input {
  flex: 1; 
}
.w-full {
  width: 100%;
}

/* MODERN BUTONLAR */
.modern-btn {
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
}

.modern-btn.primary {
  background-color: #2563eb; 
  color: white;
}
.modern-btn.primary:hover:not(:disabled) {
  background-color: #1d4ed8;
}
.modern-btn.primary.shadow {
  box-shadow: 0 4px 6px -1px rgba(37, 99, 235, 0.2), 0 2px 4px -1px rgba(37, 99, 235, 0.1);
}
.modern-btn:disabled {
  background-color: #93c5fd;
  cursor: not-allowed;
}

.modern-btn.outline {
  background-color: white;
  border: 1px solid #cbd5e1;
  color: #475569;
}
.modern-btn.outline:hover {
  background-color: #f1f5f9;
  border-color: #94a3b8;
  color: #0f172a;
}

.modern-btn.text-btn {
  background-color: transparent;
  color: #64748b;
}
.modern-btn.text-btn:hover {
  background-color: #f1f5f9;
  color: #334155;
}


/* --- MODAL TASARIMI --- */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(15, 23, 42, 0.4); /* Koyu mavimsi şık bir transparanlık */
  backdrop-filter: blur(4px); /* Arka planı hafif buzlu cam yapar (Apple tarzı) */
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: #ffffff;
  width: 420px;
  border-radius: 12px; /* Daha yumuşak köşeler */
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04); /* Derinlik veren modern gölge */
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 1px solid #f1f5f9;
}

.modal-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 700;
  color: #1e293b;
}

.close-icon {
  background: none;
  border: none;
  font-size: 24px;
  line-height: 1;
  color: #94a3b8;
  cursor: pointer;
  transition: color 0.2s;
}
.close-icon:hover {
  color: #ef4444; /* Üzerine gelince tatlı bir kırmızı */
}

.modal-body {
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group label {
  display: block;
  font-size: 13px;
  font-weight: 600;
  color: #64748b;
  margin-bottom: 6px;
}

.modal-footer {
  padding: 16px 24px;
  background-color: #f8fafc; /* Alt kısım hafif gri kalsın ki butonlar öne çıksın */
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  border-top: 1px solid #f1f5f9;
}


.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.modal-fade-enter-from .modal-content,
.modal-fade-leave-to .modal-content {
  transform: scale(0.95) translateY(15px); /* Aşağıdan ve küçülerek gelir */
}
</style>
