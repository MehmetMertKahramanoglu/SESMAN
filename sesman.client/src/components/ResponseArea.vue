<script setup lang="ts">
import { useRequestStore } from '@/Stores/requestStore';

// Pinia'ya bağlanıyoruz
const store = useRequestStore();
</script>

<template>
  <div class="response-container">
    <div class="response-header-title">
      <h3>Response</h3>
    </div>

    <!-- İstek gönderilirken -->
    <div v-if="store.isLoading" class="status-box loading">
      Request is being processed, please wait...
    </div>

    <!-- Henüz cevap yoksa -->
    <div v-else-if="!store.response" class="status-box empty">
      No request has been sent yet.
    </div>

    <!-- Response geldiyse -->
    <div v-else class="response-content">

      <!-- Status kodu ve işlem süresi -->
      <div class="response-meta">
        <span class="meta-badge">
          Status: <strong>{{ store.response.statusCode }}</strong>
        </span>
        <span class="meta-badge">
          Süre: <strong>{{ store.response.executionTimeMs }} ms</strong>
        </span>
      </div>

      <!-- Response body -->
      <div class="response-section">
        <h4>Body</h4>
        <textarea 
          readonly 
          class="response-textarea" 
          :value="store.response.body || 'No Content'"
        ></textarea>
      </div>

      <!-- Response headerları -->
      <div class="response-section" v-if="store.response.responseHeaders && store.response.responseHeaders.length > 0">
        <h4>Headers</h4>
        <table class="simple-table">
          <tbody>
            <tr v-for="(header, index) in store.response.responseHeaders" :key="index">
              <td class="fw-bold">{{ header.key }}</td>
              <td>{{ header.value }}</td>
            </tr>
          </tbody>
        </table>
      </div>

    </div>
  </div>
</template>

<style scoped>
  /* Ana kutu */
.response-container {
  border: 1px solid #ccc;
  border-radius: 4px;
  background-color: #fff;
  display: flex;
  flex-direction: column;
}

  /* Başlık alanı */
.response-header-title {
  background-color: #f8f9fa;
  padding: 10px 15px;
  border-bottom: 1px solid #ccc;
}

.response-header-title h3 {
  margin: 0;
  font-size: 16px;
  color: #333;
}

  /* Bilgilendirme kutuları */
.status-box {
  padding: 30px;
  text-align: center;
  font-size: 14px;
}
.status-box.empty { color: #6c757d; }
.status-box.loading { color: #0d6efd; font-weight: bold; }


.response-content {
  padding: 15px;
}

  /* Status ve süre bilgileri */
.response-meta {
  display: flex;
  gap: 15px;
  margin-bottom: 15px;
}
.meta-badge {
  padding: 5px 10px;
  background-color: #e9ecef;
  border-radius: 4px;
  font-size: 13px;
  color: #333;
  border: 1px solid #dee2e6;
}

  /* Body ve Header başlıkları */
.response-section h4 {
  margin: 0 0 10px 0;
  font-size: 14px;
  color: #555;
}

  /* Response body */
.response-textarea {
  width: 100%;
  height: 200px;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-family: monospace;
  resize: vertical;
  background-color: #fdfdfd;
  color: #333;
  box-sizing: border-box;
  margin-bottom: 20px;
}
.response-textarea:focus {
  outline: none;
  border-color: #ccc;
}

  /* Header tablosu */
.simple-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.simple-table td {
  padding: 8px;
  border: 1px solid #eee;
}
.fw-bold {
  font-weight: bold;
  width: 30%;
  background-color: #fafafa;
}
</style>
