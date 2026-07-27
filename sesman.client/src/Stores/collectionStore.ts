import { defineStore } from 'pinia';
import { ref } from 'vue';
import axios from 'axios';


export interface AuthConfigDto {
    type: string;
    username?: string;
    password?: string;
    token?: string;
    apiKeyName?: string;
    apiKeyValue?: string;
    apiKeyAddTo?: string;
}

// Sağ ekrandan yeni isteğin tipi 
export interface CreateSavedRequestDto {
  name: string;
  collectionId: string;
  url: string;
  method: number; 
  body?: string | null;
  bodyType?: string | null; 
  savedRequestHeaders?: { key: string; value: string }[];
  savedRequestParameters?: { key: string; value: string }[];

  auth?: AuthConfigDto;
}

// Backend'den gelen kayıtlı isteğin tipi 
export interface SavedRequest {
  id: string;
  name: string;
  collectionId: string;
  url: string;
  method: number;
  body?: string | null;
  bodyType?: string | null; 
  createdAt: string;
  savedRequestHeaders?: { key: string; value: string }[];
  savedRequestParameters?: { key: string; value: string }[];

  auth?: AuthConfigDto;
}

// Backend'den gelen Klasör tipi
export interface Collection {
  id: string;
  name: string;
  createdAt: string;
  savedRequests: SavedRequest[]; 
}

export const useCollectionStore = defineStore('collection', () => {

    // ref ile tanımlıyoruz ki veri değiştiğinde ekran anında güncellensin
    const collections = ref<Collection[]>([]);
    const isLoading = ref<boolean>(false);

    const API_BASE_URL = 'https://localhost:7076/api'; 


    // DB'deki klasörleri çekmek için
    const fetchCollections = async () => {
        isLoading.value = true;
        try {
            const response = await axios.get<Collection[]>(`${API_BASE_URL}/Collection`);
            
            collections.value = response?.data || [];
            return true;
        } catch (error) {
            console.error('An error occurred while retrieving the collections:', error);
            return false;
        } finally {
            isLoading.value = false;
        }
    };

    // Yeni klasör oluşturmak için
    const createCollection = async (name: string) => {
        try {
            await axios.post(`${API_BASE_URL}/Collection`, { name: name });
            // Ekledikten sonra güncel listeyi görmek için çağırıyorum
            await fetchCollections(); 
            return true;
        } catch (error) {
            console.error('Could not create folder:', error);
            return false;
        }
    };

    // Sağ ekranda oluşturulan isteği kaydetmek için
    //burada RequestBar tarafından gelen payload requestData'ya atanıyor. Ve CreateSavedRequestDto'in içindeki bütün değerler var mı diye kontrol ediliyor (bu kontrol amaçlı kod yazarken hata veriyor uyumsuzluk durumunda.)
    const saveNewRequest = async (requestData: CreateSavedRequestDto) => {
        try {
            await axios.post(`${API_BASE_URL}/SavedRequest`, requestData); //burada C# tarafını bekleyip devam etmesi için await yazdık. requestData SavedRequestController kısmına gider (post olduğu için [HttpPost] kısmına gider.) 
            // Backend tarafından kayıt başarılı geldikten sonra fetchCollections çağırılır
            await fetchCollections();
            //ve true dönüp RequestBar kısmındaki isSucces'i true yapar.
            return true;
        }
        catch (error) 
        {
            console.error('The request template could not be saved:', error);
            return false;
        }
    };

    // İstek silme fonksiyonu
    const deleteSavedRequest = async (folderId: string, requestId: string) => {
        
        if (!confirm("Are you sure you want to delete this template?")) {
            return false;
        }

        try {
            // Backend'e silme isteğini atıyoruz 
         await axios.delete(`https://localhost:7076/api/SavedRequest/${requestId}`);
            
            // Eğer silme işlemi başarılıysa, Vue'daki listemizdende siliyorum, sayfa yenilemeye gerek kalmaması için
          const folder = collections.value.find(c => c.id === folderId);
            
            if (folder && folder.savedRequests) {
                
                const index = folder.savedRequests.findIndex((r: { id: string }) => r.id === requestId);
                
                if (index !== -1) {
                    folder.savedRequests.splice(index, 1);
                }
            }
            
            return true;
        } catch (error) {
            console.error("An error occurred while deleting the request:", error);
            alert("The deletion process failed!");
            return false;
        }
    };


    // Dışarıda kullanabilmek için return alıyorum
    return {
        collections,
        isLoading,
        fetchCollections,
        createCollection,
        saveNewRequest,
        deleteSavedRequest
    };
});
