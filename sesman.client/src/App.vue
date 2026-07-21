<script setup lang="ts">
import SidebarMenu from '@/components/SidebarMenu.vue'
import RequestBar from '@/components/RequestBar.vue'
import RequestTabs from '@/components/RequestTabs.vue'
import ResponseArea from '@/components/ResponseArea.vue'


import { ref } from 'vue';
import { useRequestStore } from '@/Stores/requestStore';
import { useCollectionStore } from '@/Stores/collectionStore';

const requestStore = useRequestStore();
const collectionStore = useCollectionStore();

const isSaveModalOpen = ref(false);
const newRequestName = ref('');
const selectedCollectionId = ref('');

const openSaveModal = () => isSaveModalOpen.value = true;
const closeSaveModal = () => {
  isSaveModalOpen.value = false;
  newRequestName.value = '';
  selectedCollectionId.value = '';
};

const confirmSave = async () => {
  if (!newRequestName.value || !selectedCollectionId.value) {
    alert("Please enter a name and select a folder!");
    return;
  }

  // Böylece methodMap kullanmadan, direkt 'GET', 'POST' kelimesini yollayabileceğiz.
  const payload: any = {
    name: newRequestName.value,
    collectionId: selectedCollectionId.value,
    url: requestStore.url,
    
 
    method: requestStore.method, 
    
    body: requestStore.body,
    savedRequestHeaders: requestStore.requestHeaders.map(h => ({ key: h.key, value: h.value })),
    savedRequestParameters: requestStore.requestParameters.map(p => ({ key: p.key, value: p.value }))
  };

  const isSuccess = await collectionStore.saveNewRequest(payload);
  
  if (isSuccess) {
    alert("The template was successfully saved to the folder!");
    closeSaveModal(); 
  }
};
</script>

<template>
  <div class="sesman-layout">
    
    <aside class="sidebarMenu-container">
      <SidebarMenu />
    </aside>

    <main class="main-content">
      <!-- URL ve HTTP metodunun bulunduğu bölüm -->
      <RequestBar />
      <!-- Header, Params ve Body sekmeleri -->
      <RequestTabs />
      <!-- Gelen cevabın gösterildiği alan -->
      <ResponseArea />

    </main>
    
  </div>
</template>

<style>
  /* Sayfa genel ayarları */
body, html {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: Arial, sans-serif;
  background-color: #ffffff; /* Beyaz arka plan */
  color: #333333; /* Siyah yazı */
}

  /* tam ekran kaplaması için */
#app {
  width: 100vw;
  height: 100vh;
  margin: 0;
  padding: 0;
  display: flex;
}

  /* Ana sayfa düzeni */
.sesman-layout {
  display: flex;
  width: 100%;
  height: 100%;
}

  /* Sol menü */
.sidebarMenu-container {
  width: 250px;
  background-color: #f8f9fa; 
  border-right: 1px solid #ddd;
  padding: 15px;
}

  /* Sağ taraf */
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 20px;
  gap: 15px;
}
</style>
