<script setup lang="ts">
import { ref } from 'vue';
import { useRequestStore } from '@/Stores/requestStore';
import RequestBody from '@/components/RequestBody.vue' // Yeni Body bileşenimiz

// Store'a erişiyoruz
const store = useRequestStore();

// Açık olan sekmeyi tutması ve sayfa açıldığında default olarak Params sekmesi seçili gelmesi için
const activeTab = ref('Params'); 
</script>

<template>
  <div class="request-tabs-container">

    <!-- Sekme butonları -->
    <div class="tabs-header">
      <button 
        :class="{ active: activeTab === 'Params' }" 
        @click="activeTab = 'Params'"
      >
        Params
      </button>
      <button
  :class="{ active: activeTab === 'Authorization' }"
  @click="activeTab = 'Authorization'"
>
  Authorization
</button>
      <button 
        :class="{ active: activeTab === 'Headers' }" 
        @click="activeTab = 'Headers'"
      >
        Headers
      </button>
      <button 
        :class="{ active: activeTab === 'Body' }" 
        @click="activeTab = 'Body'"
      >
        Body
      </button>
    </div>

    <div class="tab-content">

      <!-- Parametre bilgileri -->
      <div v-if="activeTab === 'Params'">
        <table class="key-value-table">
          <tbody>
            <tr v-for="(param, index) in store.requestParameters" :key="index">
              <td><input v-model="param.key" placeholder="Key" class="simple-input" /></td>
              <td><input v-model="param.value" placeholder="Value" class="simple-input" /></td>
              <td>
                <button @click="store.removeRequestParameter(index)" class="delete-btn">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
        <!-- Yeni parametre ekleyen buton -->
        <button @click="store.addRequestParameter()" class="add-btn">+ Add Parameter</button>
      </div>


        <!-- Authorization kısmı -->
<div v-if="activeTab === 'Authorization'">

  <label>Type</label>

  <select v-model="store.authType" class="simple-input">
    <option value="No Auth">No Auth</option>
    <option value="Basic Auth">Basic Auth</option>
    <option value="Bearer Token">Bearer Token</option>
    <option value="OAuth 2.0">OAuth 2.0</option>
    <option value="API Key">API Key</option>
  </select>

  <!-- Bearer -->
  <div v-if="store.authType === 'Bearer Token'" class="auth-section">
    <input
      v-model="store.bearerToken"
      class="simple-input"
      placeholder="Bearer Token"
    />
  </div>

  <!-- Basic -->
  <div v-if="store.authType === 'Basic Auth'" class="auth-section">
    <input
      v-model="store.basicUsername"
      class="simple-input"
      placeholder="Username"
    />

    <input
      v-model="store.basicPassword"
      class="simple-input"
      type="password"
      placeholder="Password"
    />
  </div>

  <!-- API Key -->
  <div v-if="store.authType === 'API Key'" class="auth-section">
    <input
      v-model="store.apiKey"
      class="simple-input"
      placeholder="API Key"
    />

    <input
      v-model="store.apiKeyHeader"
      class="simple-input"
      placeholder="Header Name (e.g. x-api-key)"
    />
  </div>

  <!-- OAuth 2.0 -->
  <div v-if="store.authType === 'OAuth 2.0'" class="auth-section">
    <label>Access Token</label>
    <input
      v-model="store.oauthToken"
      class="simple-input"
      placeholder="Access Token"
    />
  </div>
  
</div>


      <!-- Header bilgileri -->
      <div v-if="activeTab === 'Headers'">
        <table class="key-value-table">
          <tbody>
            <tr v-for="(header, index) in store.requestHeaders" :key="index">
              <td><input v-model="header.key" placeholder="Key" class="simple-input" /></td>
              <td><input v-model="header.value" placeholder="Value" class="simple-input" /></td>
              <td>
                <button @click="store.removeRequestHeader(index)" class="delete-btn">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
        <button @click="store.addRequestHeader()" class="add-btn">+ Add Header</button>
      </div>

      <!--  Body Kısmı -->
      <div v-if="activeTab === 'Body'" class="body-wrapper">
        <!-- Eski textarea'yı sildik, yerine yeni tasarladığımız component'i koyduk -->
        <RequestBody />
      </div>

    </div>
  </div>
</template>

<style scoped>
/* Ana kutu */
.request-tabs-container {
  border: 1px solid #ccc;
  border-radius: 4px;
  background-color: #fff;
  display: flex;
  flex-direction: column;
}

/* Sekme butonları */
.tabs-header {
  display: flex;
  background-color: #f8f9fa;
  border-bottom: 1px solid #ccc;
}

.tabs-header button {
  padding: 10px 20px;
  border: none;
  background-color: transparent;
  cursor: pointer;
  font-size: 14px;
  border-right: 1px solid #ccc;
  color: #555;
}

/* Üzerine gelince */
.tabs-header button:hover {
  background-color: #e9ecef;
}

/* Seçili sekme */
.tabs-header button.active {
  background-color: #fff;
  font-weight: bold;
  color: #000;
  border-bottom: 2px solid #0d6efd; 
}

/* Sekme içeriği */
.tab-content {
  padding: 15px;
  flex: 1; /* İçeriğin kalan boşluğu doldurmasını sağlar */
}


.body-wrapper {
  min-height: 250px; /* İçerik için yeterli alan açar */
}

/* Key - Value tablosu */
.key-value-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 10px;
}

.key-value-table td {
  padding: 5px;
}

/* Input alanları */
.simple-input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box; 
}

/* Ekle butonu */
.add-btn {
  padding: 6px 12px;
  background-color: #198754; 
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
}
.add-btn:hover { background-color: #157347; }

/* Sil butonu */
.delete-btn {
  padding: 8px 12px;
  background-color: #dc3545; 
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
}
.delete-btn:hover { background-color: #bb2d3b; }

  .auth-section {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-top: 15px;
  }

</style>
