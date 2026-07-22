import { defineStore } from 'pinia';
//veri taşıma konusunda tek bir depo olmasını sağlıyor. Normalde propslarla her seferinde veri taşımam gerekirdi. Sonrasında emit ile verinin değiştiğinin bilgisini söylemem gerekirdi.
import { ref } from 'vue';
import axios from 'axios'; //backend ile vue arası iletişim kurmamı sağlıyor.

//backend tarafındaki Dto'lar
import type { 
    RequestHeaderDto, 
    RequestParameterDto, 
    ResponseLogDto,
} from '@/types/sesman';

// Geçmiş isteklerin tutulacağı model
export interface HistoryItemDto {
    id?: string;
    url: string;
    method: string;
    body?: string;
    bodyType?: string;
    rawType?: string; // YENİ: Geçmişte seçilen Raw tipini tutmak için
    requestHeaders?: RequestHeaderDto[];
    requestParameters?: RequestParameterDto[];
    response?: ResponseLogDto;
}

// request store (pinia sayesinde direkt 1 store oluşturdum)
export const useRequestStore = defineStore('request', () => {

    //Buradaki tanımlamalar pinia sayesinde ortak bir hafıza olmasını sağlıyor. Değişim olduğunda bildirmeme gerek kalmıyor (emit yapılmasına gerek kalmıyor).
    const url = ref<string>('');
    const method = ref<string>('GET');
    const body = ref<string>('{\n  "isim": "Mert",\n  "yas": 23\n}'); //raw için kullanılacak

    const binaryContent = ref<string>(''); // Base64'e çevrilmiş dosya içeriği

    const bodyType = ref<string>('raw'); // none, form-data, x-www-form-urlencoded, raw, binary, GraphQL /Default olarak raw başlar
    const rawType = ref<string>('JSON'); // JSON, Text, XML vb.
    
    const formDataList = ref([{ key: '', value: '', type: 'text' }]); //form-data için
    const urlEncodedList = ref([{ key: '', value: '' }]);
    const graphqlQuery = ref<string>('');
    const graphqlVariables = ref<string>('');


    const currentPage = ref<number>(1); // Şu an kaçıncı sayfadayız
    const hasMore = ref<boolean>(true); // Veritabanında daha veri kaldı mı?

    //header ve parametre kısımları için listeler
    const requestHeaders = ref<RequestHeaderDto[]>([]); 
    const requestParameters = ref<RequestParameterDto[]>([]);

    //gelen son cevap
    const response = ref<ResponseLogDto | null>(null);

    // istek gönderirken loading diye gösterebilmek için başta false tanımladım.
    const isLoading = ref<boolean>(false); 
    
    // History kısmı
    const history = ref<HistoryItemDto[]>([]);

    //bu kısımlar direkt store.sendRequest(); ile çağırılıp kullanılabiliyor
    //koyduğum değişkenlere göre header listesine yeni satır ekleme
    const addRequestHeader = () => {
        requestHeaders.value.push({ id: '', createdAt: '', updatedAt: '', key: '', value: '' });
    };

    //header kaldırmak için
    const removeRequestHeader = (index: number) => {
        requestHeaders.value.splice(index, 1);
    };

    //koyduğum değişkenlere göre params listesine yeni satır ekleme
    const addRequestParameter = () => {
        requestParameters.value.push({ id: '', createdAt: '', updatedAt: '', key: '', value: '' });
    };

    //parametreyi kaldırmak için
    const removeRequestParameter = (index: number) => {
        requestParameters.value.splice(index, 1);
    };

    //yeni istek atılacağı zaman ekranı default haline getirmek için
    const clearRequest = () => {
        url.value = '';
        method.value = 'GET';
        body.value = `{\n  "ornekDeger": "Buraya veri girin",\n  "aktifMi": true,\n  "sayi": 123\n}`;
        requestHeaders.value = [];
        requestParameters.value = [];
        response.value = null;
    };

    // DB deki geçmiş kayıtları çekmek için
    const fetchHistory = async (loadMore = false) => { 
        try {
            if (!loadMore) {
                currentPage.value = 1;
                hasMore.value = true; 
            }

            const res = await axios.get(`https://localhost:7076/api/RequestLog?page=${currentPage.value}&pageSize=10`);
            
            if (res.data.length < 10) {
                hasMore.value = false;
            }

            if (loadMore) {
                history.value.push(...res.data); 
            } else {
                history.value = res.data; 
            }
            return true; 
        } catch (error) {
            console.error("Geçmiş çekilemedi:", error);
            return false; 
        }
    };

    const loadNextPage = async () => {
        if (hasMore.value) {
            currentPage.value++; 
            await fetchHistory(true); 
        }
    };

    const loadRequest = (item: HistoryItemDto) => {
        if (!item) return;

        console.log("Geçmişten veya Klasörden Tıklanan İstek İçeriği:", item);

        url.value = item.url || '';
        method.value = item.method || 'GET';
        requestHeaders.value = item.requestHeaders || [];
        requestParameters.value = item.requestParameters || [];
        
        response.value = item.response || null;

        // Veritabanından gelen bodyType (Default olarak raw)
        bodyType.value = item.bodyType || 'raw';
        
        // YENİ: Geçmişteki Raw tipini geri yükle (Eğer yoksa JSON yap)
        rawType.value = item.rawType || 'JSON'; 
        
        if (bodyType.value === 'binary') {
            binaryContent.value = item.body || '';
        } 
        else if (bodyType.value === 'form-data') {
            try {
                formDataList.value = item.body ? JSON.parse(item.body) : [];
            } catch { 
                formDataList.value = [];
            }
        } 
        else if (bodyType.value === 'x-www-form-urlencoded') {
            try {
                urlEncodedList.value = item.body ? JSON.parse(item.body) : [];
            } catch {
                urlEncodedList.value = [];
            }
        } 
        else if (bodyType.value === 'GraphQL') {
            try {
                const parsed = item.body ? JSON.parse(item.body) : {};
                graphqlQuery.value = parsed.query || '';
                graphqlVariables.value = parsed.variables ? JSON.stringify(parsed.variables) : '';
            } catch {
                graphqlQuery.value = '';
                graphqlVariables.value = '';
            }
        } 
        else {
            // raw veya tanımsız ise doğrudan raw kutusuna bas
            body.value = item.body || ''; 
        }
    };

    // Backend tarafına istek göndermek için
    const sendRequest = async () => {
        if (!url.value) {
            alert("URL bulunamadı");
            return;
        }

        isLoading.value = true;
        
        try {
            // Kullanıcının seçtiği Body Tipine göre veriyi ayarlama
            let finalBody = ''; 

            if (bodyType.value === 'none') {
                finalBody = ''; 
            } 
            else if (bodyType.value === 'raw') {
                finalBody = body.value; 
            }
            else if (bodyType.value === 'form-data') {
                finalBody = JSON.stringify(formDataList.value);
            }
            else if (bodyType.value === 'x-www-form-urlencoded') {
                finalBody = JSON.stringify(urlEncodedList.value);
            }
            else if (bodyType.value === 'GraphQL') {
                finalBody = JSON.stringify({
                    query: graphqlQuery.value,
                    variables: graphqlVariables.value ? JSON.parse(graphqlVariables.value) : {}
                });
            }
            else if (bodyType.value === 'binary') {
                finalBody = binaryContent.value; 
            }

            // --- YENİ AŞAMA: Otomatik Content-Type Ekleme ---
            const finalHeaders = requestHeaders.value.map(h => ({ key: h.key, value: h.value }));            
            // Kullanıcı Headers sekmesinden manuel "Content-Type" girdiyse onu bozmayalım
            const hasContentType = finalHeaders.some(h => h.key.toLowerCase() === 'content-type');

            if (!hasContentType) {
                if (bodyType.value === 'raw') {
                    let mimeType = 'text/plain'; // Varsayılan

                    if (rawType.value === 'JSON') mimeType = 'application/json';
                    else if (rawType.value === 'XML') mimeType = 'application/xml';
                    else if (rawType.value === 'HTML') mimeType = 'text/html';

                    finalHeaders.push({ key: 'Content-Type', value: mimeType });
                } 
                else if (bodyType.value === 'GraphQL') {
                    finalHeaders.push({ key: 'Content-Type', value: 'application/json' });
                }
            }
            // ------------------------------------------------

            // Backend tarafına gönderilecek ana payload'u hazırlama
            const payload = {
                url: url.value,
                method: method.value,
                body: finalBody, 
                bodyType: bodyType.value,
                rawType: rawType.value, // YENİ: C#'a gönderiyoruz
                requestHeaders: finalHeaders, // YENİ: Content-Type eklenmiş listeyi yolluyoruz
                requestParameters: requestParameters.value.map(p => ({ key: p.key, value: p.value }))
            };
            
            const apiUrl = 'https://localhost:7076/api/Integration';
            const res = await axios.post(apiUrl, payload);

            response.value = res.data.response;
            await fetchHistory();
            
        } catch (error) {
            console.error("Could not communicate with the API:", error);
            alert("The backend could not be reached.");
        } finally {
            isLoading.value = false;
        }
    };

    //dışarıda kullanabilmek için return alıyorum. Pinia bu kısımları dışarıda direkt kullanmamı sağlıyor.
    return {
        url, 
        method, 
        body, 
        requestHeaders, 
        requestParameters, 
        response, 
        isLoading,
        history,
        addRequestHeader, 
        removeRequestHeader, 
        addRequestParameter, 
        removeRequestParameter, 
        clearRequest, 
        sendRequest,
        fetchHistory, 
        loadRequest,
        hasMore,
        loadNextPage,
        bodyType,
        rawType,
        formDataList,
        urlEncodedList,
        graphqlQuery,
        graphqlVariables,
        binaryContent
    };
});
