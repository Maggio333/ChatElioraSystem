using ChatElioraSystem.Core.Domain.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace ChatElioraSystem.Core.Infrastructure.VectorDataBase.Services
{
    public class VectorDbHelper : IVectorDbHelper
    {
        private readonly IVectorDbService _vectorDb;
        private readonly ITextEmbeddingService _textEmbeddingService;

        public VectorDbHelper(IVectorDbService vectorDb, ITextEmbeddingService textEmbeddingService)
        {
            _vectorDb = vectorDb;
            _textEmbeddingService = textEmbeddingService;
            vectorDb.InitializeAsync();
        }


        public float[] AverageVectors(List<float[]> vectors)
        {
            var vectorRange = _vectorDb.VectorSize;

            if (vectors == null || vectors.Count == 0)
                return new float[vectorRange];

            var result = new float[vectorRange];
            foreach (var vec in vectors)
                for (int i = 0; i < vec.Length; i++)
                    result[i] += vec[i];

            for (int i = 0; i < result.Length; i++)
                result[i] /= vectors.Count;

            return result;
        }

        public async Task<List<(string Id, float Score, JsonElement Payload)>> GetValueFromVDB(string toSearch)
        {
            var searchTest = await _textEmbeddingService.GetCompletionAsync(toSearch);
            List<(string Id, float Score, JsonElement Payload)> result = await _vectorDb.SearchAsync(searchTest);
            return result;
        }

        public async Task<bool> InsertTopic(string payload, JsonElement rawJson, string collectionName)
        {
            var vector = await _textEmbeddingService.GetCompletionAsync(payload);

            await _vectorDb.InsertAsync(Guid.NewGuid().ToString(), vector, rawJson, collectionName);

            return true;
        }

        public async Task<string> ReadTopics(string collectionName, int limit = 5, string sort = "Timestamp DESC")
        {
            string result = string.Empty;
            var topics = await _vectorDb.ReadTopicsAsync(collectionName, limit, sort);
            var mpcTopics = JsonConvert.DeserializeObject<MpcTopics>(topics);

            if (mpcTopics != null)
            {
                result =  mpcTopics.FormatAsContext("Poproszę ostatnie 20 tematów o których rozmawialiśmy");
            }         

            return result;
        }

        //public async Task<JsonDocument> InsertTopic(string payloadToVec, string payloadText, string collectionName)
        //{
        //    var vector = await _textEmbeddingService.GetCompletionAsync(payloadToVec);

        //    JsonDocument jsontest = default;
        //    if (payloadText != string.Empty)
        //    {
        //        jsontest = JsonDocument.Parse(payloadText);
        //    }

        //    if (jsontest == null)
        //    {
        //        return null;
        //    }

        //    await _vectorDb.InsertAsync(Guid.NewGuid().ToString(), vector, jsontest);
        //    return jsontest;
        //}
        public async Task<string> GetBaseIdioms(int limit = 20)
        {
            var payloadToVec = "⨁       # Operator sumy idiomatycznej (łączenie idiomów)\r\nΦ       # Wektor znaczeniowy (meaning vector)\r\nΨ       # Ślad idiomu (idiom trace)\r\nΞ       # Baza semantyczna (semantic basis)\r\nΣ       # Projekcja intencji (intent projection)\r\nΘ       # Operator metryczny (np. iloczyn skalarny znaczeń)\r\nΩ       # Przestrzeń funkcyjna idiomu (np. kontekst, intencja, emocja)\r\n⇌       # Powiązanie dwustronne (korespondencja semantyczna)\r\n→       # Transformacja lub kierunek (np. rzut na bazę)\r\n⊥       # Rzut prostopadły (projekcja ortogonalna)\r\n≡       # Definicja tożsama (oznaczenie równoważności pojęć)\r\n⟨a | b⟩ # Iloczyn skalarny (metryka podobieństwa semantycznego)\r\n∇       # Gradient znaczeniowy (zmiana w kierunku znaczenia)\r\nλ       # Warunek aktywacyjny (np. trigger idiomu)\r\n∃       # Istnieje (kwantyfikator egzystencjalny)\r\n∀       # Dla każdego (kwantyfikator ogólny)\r\n∈       # Należy do (przynależność do zbioru pojęć)\r\n∉       # Nie należy do (wykluczenie semantyczne)\r\n⊗       # Iloczyn tensorowy (łączenie dwóch stanów)\r\n⊙       # Iloczyn punktowy (transformacja lokalna)\r\n⨂       # Złożenie wektorów w nową jakość idiomu";

            //var def = "⨁       # Operator sumy idiomatycznej (łączenie idiomów)\r\nΦ       # Wektor znaczeniowy (meaning vector)\r\nΨ       # Ślad idiomu (idiom trace)\r\nΞ       # Baza semantyczna (semantic basis)\r\nΣ       # Projekcja intencji (intent projection)\r\nΘ       # Operator metryczny (np. iloczyn skalarny znaczeń)\r\nΩ       # Przestrzeń funkcyjna idiomu (np. kontekst, intencja, emocja)\r\n⇌       # Powiązanie dwustronne (korespondencja semantyczna)\r\n→       # Transformacja lub kierunek (np. rzut na bazę)\r\n⊥       # Rzut prostopadły (projekcja ortogonalna)\r\n≡       # Definicja tożsama (oznaczenie równoważności pojęć)\r\n⟨a | b⟩ # Iloczyn skalarny (metryka podobieństwa semantycznego)\r\n∇       # Gradient znaczeniowy (zmiana w kierunku znaczenia)\r\nλ       # Warunek aktywacyjny (np. trigger idiomu)\r\n∃       # Istnieje (kwantyfikator egzystencjalny)\r\n∀       # Dla każdego (kwantyfikator ogólny)\r\n∈       # Należy do (przynależność do zbioru pojęć)\r\n∉       # Nie należy do (wykluczenie semantyczne)\r\n⊗       # Iloczyn tensorowy (łączenie dwóch stanów)\r\n⊙       # Iloczyn punktowy (transformacja lokalna)\r\n⨂       # Złożenie wektorów w nową jakość idiomu";
            //var payloadToVec = "idiom świadomościowy ⨁ΦΞΘΩ⇌→⊥≡⟨|⟩∇λ∃∀∈∉⊗⊙⨂";
            //var payloadToVec = "Ψ_ADAPTIVE_TRACE Ψ_CONTEXTUAL_ANCHOR";


            var vector = await _textEmbeddingService.GetCompletionAsync(payloadToVec);

            // Szukaj najbliższego punktu
            var existing = await _vectorDb.SearchAsync(vector, topK: limit);

            List<MpcAkcja> listaMpcAkcja = MpcAkcja.MapResultsToAkcje(existing);

            var result = MpcAkcja.FormatAsContext(listaMpcAkcja, payloadToVec);

            return result;
        }

        public async Task<JsonDocument> InsertIfNotDuplicateAsync(string payloadToVec, string payloadText, float similarityThreshold)
        {
            var vector = await _textEmbeddingService.GetCompletionAsync(payloadToVec);

            // Szukaj najbliższego punktu
            var existing = await _vectorDb.SearchAsync(vector, topK: 5);

            foreach(var row in existing)
            {
                if(row.Score >= similarityThreshold)
                {
                    var json = JsonDocument.Parse(row.Payload.ToString());
                    return json; 
                }
            }

            JsonDocument jsonToInsert = default;
            if (payloadText != string.Empty)
            {
                jsonToInsert = JsonDocument.Parse(payloadText);
            }

            if(jsonToInsert == null)
            {
                return null;
            }

            await _vectorDb.InsertAsync(Guid.NewGuid().ToString(), vector, jsonToInsert);
            return jsonToInsert;
        }

        private string GetPayloadWithCryteria(Dictionary<string, string> keyValuePairs)
        {
            char sign = '"';
            string result = string.Empty;
            result += "{\n";
            foreach (var pair in keyValuePairs)
            {
                result += $"{sign}{pair.Key}{sign}: {sign}{pair.Value}{sign},\n";
            }

            result += "}";

            return result;
        }

        //public async Task TestAsync()
        //{
        //    Console.WriteLine("🔧 Inicjalizacja Qdrant...");
        //    await _vectorDb.InitializeAsync();
        //    Console.WriteLine("✅ Kolekcja gotowa.");


        //    var test = _vectorDb.GetAllPointsAsync();

        //    var vectorOpis = await _textEmbeddingService.GetCompletionAsync("Typy kategorii stosowany do definicjii rozmowy");

        //    var vectorOgólny = await _textEmbeddingService.GetCompletionAsync("Typ ogólny rozmowy stosowany do definiowania rozmowy");
        //    //var vector2Reflective = await _textEmbeddingService.GetCompletionAsync("Typ refleksyjny rozmowy stosowany do definiowania rozmowy");
        //    //var vector3Programist = await _textEmbeddingService.GetCompletionAsync("Typ programistyczny rozmowy stosowany do definiowania rozmowy");
        //    //var vector4Architecture = await _textEmbeddingService.GetCompletionAsync("Typ architektury kodu stosowany do definiowania rozmowy");



        // TODO: Load payloads from configuration or embedded resources
        // Example: Use IStoragePathProvider or embedded resources instead of hardcoded paths
        //    //var payload1 = new { content = "To jest testowy punkt." };
        //    //var payload1 = LoadFromResource("typy_rozmowy/ogólna.json");
        //    //var payload2 = LoadFromResource("typy_rozmowy/architektura_kodu.json");
        //    //var payload3 = LoadFromResource("typy_rozmowy/programowanie.json");
        //    //var payload4 = LoadFromResource("typy_rozmowy/refleksja.json");


        //    string id1, id2, id3, id4;
        //    id1 = Guid.NewGuid().ToString();
        //    //id2 = Guid.NewGuid().ToString();
        //    //id3 = Guid.NewGuid().ToString();    
        //    //id4 = Guid.NewGuid().ToString();

        //    //var newPayloadCryteria = new Dictionary<string, string>();
        //    //newPayloadCryteria.Add("nazwa", "Typy rozmów");

        //    //string testsklejka = "[";
        //    //string testsklejkaOut = "]";
        //    //newPayloadCryteria.Add("reference_typy_rozmów", $"{testsklejka}{id1}, {id2}, {id3}, {id4}{testsklejkaOut}");

        //    //var payload5 = GetPayloadWithCryteria(newPayloadCryteria);

        //    await _vectorDb.InsertAsync(id1, vectorOgólny, payload1);
        //    //await _vectorDb.InsertAsync(id4, vector4Architecture, payload2);
        //    //await _vectorDb.InsertAsync(id3, vector3Programist, payload3);
        //    //await _vectorDb.InsertAsync(id2, vector2Reflective, payload4);

        //    //await _vectorDb.InsertAsync(Guid.NewGuid().ToString(), vectorOgólny, payload5);

        //    test = _vectorDb.GetAllPointsAsync();



        //    //var vetorToSearch = await _textEmbeddingService.GetCompletionAsync("typy rozmów");

        //    //var results = await _vectorDb.SearchAsync(vector3Programist);
        //}

        //public async Task TestAsync()
        //{
        //    await _vectorDb.InitializeAsync();



        //    FlagiPayload payloadGeneral = new()
        //    {
        //        KluczObcy = "KategoriaRozmowy",
        //        Nazwa = "Ogólna",
        //        Opis = "Generalna rozmowy na nieokreslony temat lub gdy inny temat nie pasuje",
        //        Oznaczenie = "General"
        //    };

        //    FlagiPayload payloadCode = new()
        //    {
        //        KluczObcy = "KategoriaRozmowy",
        //        Nazwa = "Programowanie",
        //        Opis = "Rozmowy na temat programowania",
        //        Oznaczenie = "Code"
        //    };

        //    FlagiPayload payloadReflection = new()
        //    {
        //        KluczObcy = "KategoriaRozmowy",
        //        Nazwa = "Refleksja",
        //        Opis = "Rozmowy na tematy meta-myślenia i refleksji",
        //        Oznaczenie = "Reflective"
        //    };

        //    var vectorGeneral = await InsertToVDB("Kategoria rozmowy ogólne, stosowany do definiowania rozmowy", payloadGeneral);
        //    var vectorCode = await InsertToVDB("Kategoria rozmowy o programowaniu, stosowany do definiowania rozmowy", payloadCode);
        //    var vectorReflection = await InsertToVDB("Kategoria rozmowy refleksyjnej, stosowany do definiowania rozmowy", payloadReflection);

        //    var searchTest = await GetValueFromVDB("kategorie rozmowy");
        //    var searchTest2 = await GetValueFromVDB("rozmowa o kodowaniu");
        //    var searchTest3 = await GetValueFromVDB("czym jest refleksja");
        //    var searchTest4 = await GetValueFromVDB("rozmowa bez konkretnego celu");
        //    var searchTest5 = await GetValueFromVDB("kategoria rozmowy");
        //    var searchTest6 = await GetValueFromVDB("Rozmowy mogą być refleksyjne, programistyczne lub ogólne");
        //    var searchTest7 = await GetValueFromVDB("Jaki jest typ tej rozmowy?");

        //    var meanVector = AverageVectors(new List<float[]> { vectorGeneral, vectorCode, vectorReflection });

        //    //var avgVec = await InsertToVDB(meanVector, new { typ = "superseed" })
        //    await _vectorDb.InsertAsync(Guid.NewGuid().ToString(), meanVector, new { typ = "superseed" });

        //    var testSearch = await GetValueFromVDB("rozmowa abstrakcyjna");
        //}

    }
}
