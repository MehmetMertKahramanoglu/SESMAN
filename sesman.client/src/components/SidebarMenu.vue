<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRequestStore } from '@/Stores/requestStore';
import { useCollectionStore } from '@/Stores/collectionStore';

const store = useRequestStore();
const collectionStore = useCollectionStore();

const activeTab = ref('history'); 

const fetchWithRetry = (fetchFn: () => Promise<Boolean>, errorMessage: string, delay = 5000 , maxTry = 5) => {
  return new Promise((resolve) => {
    let currentTry = 0;
    const attempt = async () => {
      currentTry++;
      // Fonksiyonu çalıştır ve sonucunu (true/false) bekle
      const isSuccess = await fetchFn();
      
      if (isSuccess) {
        // Başarılıysa Promise'i tamamla (resolve)
        resolve(true); 
      }
      else if(currentTry >= maxTry) {
        // Başarısızsa bekle ve recursive (kendi kendini) olarak tekrar çağır
        console.warn(`${errorMessage}. Maksimum deneme sınırına (${maxTry}) ulaşıldı. İşlem sonlandırılıyor.`);
        //Backend'in çöktüğünü varsayarak denemeyi bırakıyoruz.
        resolve(false);
        
      }
      else {
         console.log(`${errorMessage}, ${delay / 1000} saniye sonra tekrar deneniyor...`);
        setTimeout(attempt, delay);
      }
    };

    attempt(); // İlk denemeyi başlat
  });
};

onMounted(async () => {
  const historyPromise = fetchWithRetry(
    () => store.fetchHistory(), 
    "The history could not be retrieved."
  );

  const collectionsPromise = fetchWithRetry(
    () => collectionStore.fetchCollections(), 
    "Folders could not be retrieved."
  );
  });


// Yeni Klasör Ekleme Fonksiyonu
const createNewFolder = async () => {
  // Tarayıcının yerleşik kutusunu kullanarak kullanıcıdan isim istiyoruz
  const folderName = prompt("Enter the name of the new folder to be created:");
  
  // Eğer kullanıcı "İptal"e basmadıysa ve boşluk girmediyse backend'e yolla
  if (folderName && folderName.trim() !== "") {
    await collectionStore.createCollection(folderName.trim());
  }
};



// Klasörün içindeki isteğe tıklanınca sağ ekrana yükleyecek fonksiyon
const loadSavedRequest = (req: any) => {

  const formattedReq = {
    ...req,

    method: req.method,
    //  Backend'den gelen savedRequestHeaders'ı  Vue'nun "requestHeaders'ına kopyalama kısmı 
    requestHeaders: req.savedRequestHeaders || [],
    //  Backend'den gelen savedRequestParameters'ı  Vue'nun "requestParameters'ına kopyalama kısmı 
    requestParameters: req.savedRequestParameters || []

  };
  
  store.loadRequest(formattedReq);
};
</script>

<template>
  <div class="sidebar-wrapper">
    <div class="sidebar-tabs">
      <button 
         :class="['tab-btn', { active: activeTab === 'history' }]" 
         @click="activeTab = 'history'"
       >
         History
       </button>
 
      <button
         :class="['tab-btn', { active: activeTab === 'collections'}]"
         @click="activeTab = 'collections'"
       >
        Collections
      </button>
    </div>
 
    <!-- HISTORY SEKMESİ -->
    <div v-if="activeTab === 'history'" class="tab-content">  
      <div v-if="store.history.length === 0" class="empty-text">
        History loading...
      </div>
 
      <div class="history-list">
         <div 
           v-for="item in store.history"  
           :key="item.id" 
           class="history-item" 
           title="Resend this request"
           @click="store.loadRequest(item)" 
         >
           <span :class="['method-badge', (item.method || 'GET').toLowerCase()]">
             {{ item.method || 'GET' }}
           </span>
           <span class="url-text">{{ item.url }}</span>
         </div>
         
         <button 
           v-if="store.hasMore" 
           class="show-more-btn" 
           @click="store.loadNextPage"
         >
           Show More
         </button>
       </div>
     </div>
 
     <!-- COLLECTIONS SEKMESİ -->
     <div v-if="activeTab === 'collections'" class="tab-content">
       
       <!-- Butona tıklandığında createNewFolder fonksiyonunu çağır -->
       <button class="new-collection-btn" @click="createNewFolder">
         + New Collection
       </button>
 
       <!-- Yüklenme durumu -->
       <div v-if="collectionStore.isLoading" class="empty-text">
        Folders are loading...
       </div>

       <!-- GERÇEK VERİ DÖNGÜSÜ -->
       <div v-else class="history-list">
         
         <!-- Klasör Döngüsü -->
         <div v-for="folder in collectionStore.collections" :key="folder.id" class="collection-folder">
           
           <div class="folder-header">
             
             <!-- Klasör Adı -->
             <strong>{{ folder.name }}</strong> 
           </div>
           
         <div 
               v-for="req in folder.savedRequests" 
               :key="req.id"
               class="history-item"
               @click="loadSavedRequest(req)"
               title="Upload this template to your workspace."
             >
               <!-- Metodu C#'tan gelen numaraya göre String'e çevir -->
               <span :class="['method-badge', (req.method || 'GET').toLowerCase()]">
                 {{ req.method || 'GET' }}
               </span>
               
               <!-- Ekranda İsteğin Adını göster -->
               <span class="url-text">{{ req.name || req.url }}</span>

               <!-- SİLME BUTONU (Sadece üzerine gelince görünür) -->
               <button 
                 class="delete-req-btn" 
                 @click.stop="collectionStore.deleteSavedRequest(folder.id, req.id)"
                 title="Delete the request"
               >
                 🗑️
               </button>
             </div>
         </div>
       </div>
 
     </div>
   </div>
 </template>

<style scoped>

.sidebar-wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
}

/* SEKME TASARIMLARI */
.sidebar-tabs {
  display: flex;
  border-bottom: 1px solid #ddd;
  background-color: #f8f9fa;
}

.tab-btn {
  flex: 1;
  padding: 12px 0;
  border: none;
  background: none;
  font-weight: 600;
  color: #6c757d;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all 0.2s ease;
}

.tab-btn:hover {
  color: #333;
}

.tab-btn.active {
  color: #0d6efd;
  border-bottom: 2px solid #0d6efd;
  background-color: #fff;
}

.tab-content {
  padding: 10px 0;
  display: flex;
  flex-direction: column;
  flex: 1;
  overflow-y: hidden;
}

/* COLLECTIONS TASARIMI */
.new-collection-btn {
  background-color: #fff;
  border: 1px dashed #ccc;
  padding: 8px;
  border-radius: 4px;
  color: #555;
  cursor: pointer;
  margin-bottom: 15px;
  font-weight: bold;

}
.new-collection-btn:hover {
  border-color: #0d6efd;
  color: #0d6efd;
}

.collection-folder {
  margin-bottom: 10px;
}

.folder-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px;
  background-color: #f1f3f5;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  color: #333;
}
.folder-header:hover {
  background-color: #e9ecef;
}
.folder-items {
  padding-left: 15px;
  margin-top: 5px;
  display: flex;
  flex-direction: column;
  gap: 5px;
}

/* HISTORY TASARIMLARI  */
.history-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  overflow-y: auto;
}
.history-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px;
  background-color: #fff;
  border: 1px solid #eee;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s;

}
.history-item:hover {
  background-color: #e9ecef;
  border-color: #ccc;
}
.method-badge {
  font-size: 11px;
  font-weight: bold;
  width: 45px;
  text-align: left;
}
.method-badge.get { color: #0d6efd; }
.method-badge.post { color: #198754; }
.method-badge.put { color: #fd7e14; }
.method-badge.delete { color: #dc3545; }
.method-badge.patch { color: #6f42c1; }

.url-text {
  font-size: 12px;
  color: #555;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  flex: 1;
}
.empty-text {
  font-size: 13px;
  color: #888;
  text-align: center;
  margin-top: 20px;
}
.show-more-btn {
  margin-top: 10px;
  padding: 8px;
  background-color: #f8f9fa;
  border: 1px solid #ddd;
  border-radius: 4px;
  color: #0d6efd;
  font-weight: bold;
  cursor: pointer;
  width: 100%;
}
.show-more-btn:hover {
  background-color: #e2e6ea;
}
  /* İsteği Silme Butonu (Varsayılan olarak gizli) */
  .delete-req-btn {
    background: none;
    border: none;
    color: #ef4444; 
    font-size: 14px;
    cursor: pointer;
    opacity: 0; /* Başlangıçta görünmez */
    transform: translateX(10px); /* Sağda hafif kaymış durur */
    transition: all 0.2s ease;
    padding: 4px;
    border-radius: 4px;
  }

    .delete-req-btn:hover {
      background-color: #fee2e2; 
    }

  /* Mouse satırın üzerine geldiğinde butonu göster! */
  .history-item:hover .delete-req-btn {
    opacity: 1; /* Görünür yap */
    transform: translateX(0); /* Yerine oturt */
  }

  /** {
    outline: 1px solid red; BU KISMI TASARIMDA KİMİN NE KADAR YER KAPLADIĞINI GÖRMEK İÇİN KULLANIYORUM!! (CSS BOZULDU)
  }*/

</style>
