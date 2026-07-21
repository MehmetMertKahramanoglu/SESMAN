<script setup lang="ts">
import { useRequestStore } from '@/Stores/requestStore';

// Sadece store'u çağırıyoruz
const store = useRequestStore();

// Yeni satır ekleme fonksiyonları doğrudan store'daki listeleri günceller
const addFormDataRow = () => store.formDataList.push({ key: '', value: '', type: 'text' });
const addUrlEncodedRow = () => store.urlEncodedList.push({ key: '', value: '' });


// Binary dosya seçildiğinde çalışacak fonksiyon
const onBinaryFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement; //burada ts kısmına bunun HTML input nesnesi olduğu bildiriyoruz.
  const file = target.files?.[0]; //ilk dosyayı alır, binary kısmı sadece tek dosya yollamak için olduğundan dolayı diğer dosyalar olsa bile yok sayılır. Birden fazla dosya yollanması için form- data kullanılır.
  
  if (file) {
   
    const maxSizeInMB = 5; //max mb sınırı
    const maxSizeInBytes = maxSizeInMB * 1024 * 1024; //mb dönüşümü

    if (file.size > maxSizeInBytes) {
      alert(`Error: File is too large! Please select a file smaller than ${maxSizeInMB} MB.`);
      
      target.value = ''; // Ekranda dosya seçilmiş gibi görünmesin diye input'u sıfırlıyoruz
      store.binaryContent = ''; // Store'u temizliyoruz
      return; 
    }

    const reader = new FileReader(); //dosya okuma moturunu çalıştırır

    reader.onload = (e) => {  //dosya okuma bittiğinde çalışmaya başlar

      const result = e.target?.result as string; //tarayıcı dosyayı okuduğunda uzun bir metin çıktısı verir.
      
      if (result) {
        const base64String = result.split(',')[1] || ''; //verilen uzun metindeki başta olan dosyanın ön bilgisine ihtiyaç olmadığı için virgülden ikiye bölüp 1. indexi alıyoruz. Ve ts için virgülden sonrası yoksa boş metin al diyoruz.
        store.binaryContent = base64String; //son olarak bu Base64 şifresini store.binaryContent içine koyuyoruz. (asenkron çalışma var)
      }
    };
    reader.readAsDataURL(file); //burada Base64'e çevrilir
  } else {
    store.binaryContent = ''; 
  }
};

// Form-Data içindeki dosya seçildiğinde çalışacak fonksiyon
const onFormDataFileChange = (event: Event, index: number) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  
  // O anki satırı sabit bir referansa atıyoruz.
  const row = store.formDataList[index];
  
  // Eğer satır yoksa direkt çıkıyoruz.
  if (!row) return; 

  if (file) {
    const maxSizeInMB = 5; // max boyut
    const maxSizeInBytes = maxSizeInMB * 1024 * 1024; //mb dönüşümü

    if (file.size > maxSizeInBytes) {
      alert(`Error: File is too large! Please select a file smaller than ${maxSizeInMB} MB.`);
      
      target.value = ''; // Ekranda dosya adı yazılı kalmasın diye input'u temizle
      row.value = '';    // Store'da o satırın içini temizle (eski bozuk veri kalmaması için)
      return;            
    }


    const reader = new FileReader();
    reader.onload = (e) => {
      // FileReader'ın sonucunu kesin olarak belirtiyoruz
      const result = (e.target as FileReader)?.result as string; 
      
      if (result) {
        const base64String = result.split(',')[1] || ''; 
       
        row.value = base64String;
      }
    };
    reader.readAsDataURL(file);
  } else {
    row.value = '';
  }
};

</script>

<template>
  <div class="body-tab-container">

    <!-- ÜST BAR: Body Tipleri Seçimi -->
    <div class="body-type-nav">
      <label class="radio-label">
        <input type="radio" value="none" v-model="store.bodyType" /> none
      </label>
      <label class="radio-label">
        <input type="radio" value="form-data" v-model="store.bodyType" /> form-data
      </label>
      <label class="radio-label">
        <input type="radio" value="x-www-form-urlencoded" v-model="store.bodyType" /> x-www-form-urlencoded
      </label>
      <label class="radio-label">
        <input type="radio" value="raw" v-model="store.bodyType" /> raw
      </label>
      <label class="radio-label">
        <input type="radio" value="binary" v-model="store.bodyType" /> binary
      </label>
      <label class="radio-label">
        <input type="radio" value="GraphQL" v-model="store.bodyType" /> GraphQL
      </label>

      <!-- Eğer RAW seçiliyse yanda JSON/Text vs çıksın -->
      <select v-if="store.bodyType === 'raw'" v-model="store.rawType" class="raw-type-select">
        <option value="Text">Text</option>
        <option value="JSON">JSON</option>
        <option value="HTML">HTML</option>
        <option value="XML">XML</option>
      </select>
    </div>

    <!-- ALT İÇERİK: Seçime Göre Değişen Alan -->
    <div class="body-content-area">

      <!-- NONE body gönderilmeyince bilgilendirme mesajını gösterilir -->
      <div v-if="store.bodyType === 'none'" class="empty-state">
        This request does not contain a body.
      </div>

      <!-- FORM-DATA -->
      <div v-else-if="store.bodyType === 'form-data'" class="key-value-area">
        <!-- From-data listesindeki her eleman için bir satır oluşturur.-->
        <div v-for="(item, index) in store.formDataList" :key="index" class="kv-row">
          <!-- parametre adı-->
          <input v-model="item.key" placeholder="Key" class="kv-input" />

          <!-- text veya file seçimi yapılan kısım-->
          <select v-model="item.type" class="kv-type">
            <option value="text">Text</option>
            <option value="file">File</option>
          </select>

          <!-- text seçildiğinde gösterilecek alan-->
          <input v-if="item.type === 'text'" v-model="item.value" placeholder="Value" class="kv-input" />
          <!-- file seçildiğinde gösterilecek alan-->
          <input v-else type="file" class="kv-input file-input" @change="(e) => onFormDataFileChange(e, index)" />
        </div>
        <!-- yeni form data satırı eklemek için-->
        <button class="add-row-btn" @click="addFormDataRow">+ Add</button>
      </div>

      <!-- X-WWW-FORM-URLENCODED -->
      <div v-else-if="store.bodyType === 'x-www-form-urlencoded'" class="key-value-area">
        <!-- Her key-value için bir satır oluşturur.-->
        <div v-for="(item, index) in store.urlEncodedList" :key="index" class="kv-row">
          <input v-model="item.key" placeholder="Key" class="kv-input" />
          <input v-model="item.value" placeholder="Value" class="kv-input" />
        </div>
        <!-- yeni satır eklemek için buton-->
        <button class="add-row-btn" @click="addUrlEncodedRow">+ Add</button>
      </div>

      <!-- RAW -->
      <div v-else-if="store.bodyType === 'raw'" class="raw-area">
        <textarea 
          v-model="store.body"
          class="raw-textarea"
          placeholder="Enter your body data here..."></textarea>
      </div>

      <!-- BINARY -->
      <div v-else-if="store.bodyType === 'binary'" class="binary-area">
        <div class="empty-state">
          <input type="file" @change="onBinaryFileChange" />
        </div>
      </div>

      <!-- GRAPHQL -->
      <div v-else-if="store.bodyType === 'GraphQL'" class="graphql-area">
        <textarea 
          v-model="store.graphqlQuery" 
          placeholder="Query" 
          class="raw-textarea graphql-query"></textarea>
        
        <textarea 
          v-model="store.graphqlVariables" 
          placeholder="Variables (JSON)" 
          class="raw-textarea graphql-vars"></textarea>
      </div>

    </div>
  </div>
</template>

<style scoped>

.body-tab-container {
  display: flex;
  flex-direction: column;
  gap: 15px;
  height: 100%;
}

/* ÜST MENÜ */
.body-type-nav {
  display: flex;
  align-items: center;
  gap: 15px;
  border-bottom: 1px solid #e2e8f0;
  padding-bottom: 10px;
}

.radio-label {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 13px;
  color: #475569;
  cursor: pointer;
}

.radio-label input {
  cursor: pointer;
}

.raw-type-select {
  margin-left: auto;
  color: #2563eb;
  font-weight: 600;
  border: none;
  background: transparent;
  outline: none;
  cursor: pointer;
}

/* ALT İÇERİK ALANI */
.body-content-area {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* NONE & BOŞ DURUMLAR */
.empty-state {
  color: #94a3b8;
  font-size: 14px;
  text-align: center;
  padding: 40px;
  border: 1px dashed #cbd5e1;
  border-radius: 8px;
}

/* RAW & TEXTAREA */
.raw-area {
  flex: 1;
  display: flex;
}

.raw-textarea {
  flex: 1;
  width: 100%;
  min-height: 200px;
  padding: 12px;
  font-family: 'Courier New', Courier, monospace;
  font-size: 13px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  background-color: #f8fafc;
  resize: vertical;
  outline: none;
}

.raw-textarea:focus {
  border-color: #3b82f6;
}

/* KEY-VALUE (FORM-DATA vb) TABLOLARI */
.key-value-area {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.kv-row {
  display: flex;
  gap: 8px;
}

.kv-input {
  flex: 1;
  padding: 8px 12px;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
}

.kv-input:focus {
  border-color: #3b82f6;
}

.kv-type {
  padding: 8px;
  border: 1px solid #e2e8f0;
  border-radius: 4px;
  font-size: 13px;
  background-color: #f8fafc;
}

.add-row-btn {
  align-self: flex-start;
  background: none;
  border: none;
  color: #2563eb;
  font-weight: 600;
  cursor: pointer;
  padding: 8px 0;
}

.add-row-btn:hover {
  text-decoration: underline;
}

/* GRAPHQL ALANI */
.graphql-area {
  display: flex;
  flex-direction: column;
  gap: 10px;
  height: 100%;
}

.graphql-query {
  flex: 2;
}

.graphql-vars {
  flex: 1;
}
</style>
