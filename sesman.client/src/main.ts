import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'

// Vue uygulamasını oluştur
const app = createApp(App)

// Pinia altyapısını oluştur
const pinia = createPinia()

// Pinia'yı Vue'nun içine monte et 
app.use(pinia)

// Uygulamayı ekrana bas
app.mount('#app')
