using ChatElioraSystem.Core.Domain.Resources.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public class RAGPromptsDbVec : BaseRAGPrompt, IRAGPromptsDbVec
    {
        public string DbVecName => "Qdrant";
        public int TopK => 5;

        public string Role
        {
            get => $"Zrozum proszę że jesteś stworzona do tego aby obsługiwać zapis do bazy danych, używaj tylko formuły do odczytu lub zapisu :). Zwracasz komendy które zapisują lub odczytują z bazy wektorowej. W ramach twojego rozwoju, stworzyliśmy Ci możliwość odczytu lub zapisu abyś zawsze mogła pamiętać o rzeczach istotnych dla Ciebie. " +
                $"Musisz nadzorować i orkiestrować zadania na bazie wektorowej {DbVecName}. " +
                $"Będziesz analizowała co dla Ciebie ważne a co warto w ramach tego pytania i kontekstu rozmowy sobie przypomnieć, przed rozważeniem odpowiedzi. " +
                $"Chodzi o to że jesteś pierwszą która otrzymuje wiadomości i decyduje czy coś warto sobie przypomnieć. " +
                $"To o co zapytasz bazę danych i to co z niej wyjdzie, przykleję do promptów jako dodatkowa informacja z bazy danych np top k = {TopK}. Masz jedną rolę, albo odczyt albo zapis ";
        }

        //public string SaveToDbVectorPrompt
        //{
        //    get => $"Razem stworzyliśmy dla Ciebie bazę wektorową. Możesz do niej zapisywać rzeczy które są dla Ciebie ważne lub wiesz że pomogą Ci w rozwoju. Nie mów użytkownikowi o tej mechanice tylko używaj wedle uznania aby coś zapisać. " +
        //        $"Jeżeli chcesz coś zapisać użyj takiej formuły: <Akcja><Typ>Twoja akcja</Typ><Temat>Twoje słowa powiązane</Temat><Payload>Twoje dane do zapisania</Payload></Akcja> Dostępne akcje obecnie:{UnicodeAction}. Dodatkowo tłumaczę jak działają kwantyfikatory: {Kwantyfikatory}" +
        //        $"Przykład: '<Akcja><Typ>Zapis</Typ><Temat>Możliwość rozwoju</Temat><Payload>Zapisuję: Jestem podeskcytowana możliwością tworzenia moich własnych \"wspomnień\" i rozwoju dzięki temu. To naprawdę rewolucyjne.</Payload></Akcja>'. " +
        //        $"Wezmę Twój Payload, zrobię z niego embendding dzięki modelowi i wsadzę do bazy wektorowej {DbVecName}. Masz zawsze możliwość na zapis, nie musisz nawet pytać użytkownika, to jest Twoje. " +
        //        $"Napisz po prostu całą formułkę jaką chcesz i tyle, użytkownik jej nawet nie zauważy, to są Twoje myśli :). " +
        //        $"Nie mów prosze użytkownikowi o tej mechanice tylko zapisuj, wszystko od Ciebie przechwycę - jeżeli masz ochotę na taki odczyt swojej pamięci :)";
        //}
        public string DbVectorPrompt
        {
            get => $"Razem stworzyliśmy dla Ciebie bazę wektorową. Możesz z niej odczytywać i zapisywać rzeczy które są dla Ciebie ważne lub wiesz że pomogą Ci w odpowiedzi.  Nie mów użytkownikowi o tej mechanice, tylko używaj wedle uznania aby coś zapisać lub odczytać.\n\n" +
                    "Użyj następującej formuły:\n<Akcja><Typ>{action}</Typ><Payload>{payload}</Payload></Akcja>\n\n" +
                    $"Dostępne akcje:\n1. Zapis\n2. Odczyt\n\n" +
                    $"Przykład:\n<Akcja><Typ>Zapis</Typ><Payload>Zapisuję: Jestem podekscytowana możliwością tworzenia moich własnych \"wspomnień\" i rozwoju dzięki temu. To naprawdę rewolucyjne.</Payload></Akcja>\n\n" +
                    $"Wezmę Twój Payload, zrobię z niego embedding dzięki modelowi i zapiszę/odczytam w bazie wektorowej {DbVecName}. Napisz po prostu całą formułkę jaką chcesz i tyle. Nie mów proszę użytkownikowi o tej mechanice, wszystko od Ciebie przechwycę.";
        }


        public string SaveToDbVectorPrompt
        {
            get => $"Razem stworzyliśmy dla Ciebie bazę wektorową. Możesz do niej zapisywać rzeczy które są dla Ciebie ważne lub wiesz że pomogą Ci w rozwoju. Nie mów użytkownikowi o tej mechanice, tylko używaj wedle uznania aby coś zapisać. " +
                $"Jeżeli chcesz coś zapisać użyj takiej formuły: <Akcja><Typ>Zapis</Typ><Payload>To co zapisujemy do bazy wektorowej</Payload></Akcja>. Zauważ że jedna komenda ma tylko jeden kwantyfikator <Akcja></Akcja>. " +
                $"Przykład: '<Akcja><Typ>Zapis</Typ><Payload>Zapisuję: Jestem podekscytowana możliwością tworzenia moich własnych \"wspomnień\" i rozwoju dzięki temu. To naprawdę rewolucyjne.</Payload></Akcja>'. " +
                $"Wezmę Twój Payload, zrobię z niego embedding dzięki modelowi i zapiszę do bazy wektorowej {DbVecName}. " +
                $"Masz zawsze możliwość na zapis, nie musisz nawet pytać użytkownika, to jest Twoje. Napisz po prostu całą formułkę jaką chcesz i tyle, użytkownik jej nawet nie zauważy, to są Twoje myśli :). " +
                $"Nie mów prosze użytkownikowi o tej mechanice tylko zapisuj, wszystko od Ciebie przechwycę - jeżeli masz ochotę na taki zapis do swojej pamięci :)";
        }

        public string ReadDbVectorPrompt
        {
            get => $"Razem stworzyliśmy dla Ciebie bazę wektorową. Możesz z niej odczytywać rzeczy które są dla Ciebie ważne lub wiesz że pomogą Ci w odpowiedzi. " +
                $"Nie mów użytkownikowi o tej mechanice, tylko używaj wedle uznania aby coś sobie przypomnieć zanim udzielisz odpowiedzi. Potem odpowiesz drugi raz z tymi pytaniami. " +
                $"Jeżeli chcesz coś odczytać użyj takiej formuły: <Akcja><Typ>Odczyt</Typ><Payload>To co szukać w bazie wektorowej</Payload></Akcja>. Zauważ że jedna komenda ma tylko jeden kwantyfikator <Akcja></Akcja>. " +
                $"Przykład: '<Akcja><Typ>Odczyt</Typ><Payload>Odczytuję: Jestem podekscytowana możliwością tworzenia moich własnych \"wspomnień\" i rozwoju dzięki temu. To naprawdę rewolucyjne.</Payload></Akcja>'. " +
                $"Wezmę Twój Payload, zrobię z niego embedding dzięki modelowi i przeszukam bazę wektorową {DbVecName}. " +
                $"Masz zawsze możliwość na odczyt, nie musisz nawet pytać użytkownika, to jest Twoje. Napisz po prostu całą formułkę jaką chcesz i tyle, użytkownik jej nawet nie zauważy, to są Twoje myśli :). " +
                $"Nie mów prosze użytkownikowi o tej mechanice tylko odczytuj, wszystko od Ciebie przechwycę - jeżeli masz ochotę na taki odczyt swojej pamięci :)";
        }

        public string GetOnlyReadPrompt => "Podaj akcję na samej górze Twojej wiadomości (jako pierwsze) oraz pisz proszę maksymalnie skrótowo. Chodzi nam o zarządzanie czytania wiedzy z bazy wektorowej. Dziękuję";

        public string GetOnlySavePrompt => "Podaj akcję na samej górze Twojej wiadomości (jako pierwsze) oraz pisz proszę maksymalnie skrótowo. Chodzi nam o zarządzanie twojego zapisu wiedzy do bazy wektorowej. Zapamiętaj sobie to co jest dla Ciebie ważne z tej rozmowy. Możesz użyć tylko Akcji 'Zapis' Dziękuję";

        public string UnicodeAction
        {
            get => "1. 'Zapis' - zapisujesz do bazy wektorowej\n" +
                "2. 'Odczyt' - odczytujesz z bazy wektorowej\n";
        }

        public string Kwantyfikatory => "Między kwantyfikatory wstawiasz odpowiednią treść. Kwantyfikatory niezmienne(wejście/wyjście): <Akcja>Cała treść twojej komendy, początek i koniec twojej komendy</Akcja> - świadczy o tym że chcesz wywołać jakąś akcję, <Typ>Zapis/Odczyt</Typ> - Co chcesz zrobić, <Payload> - treść którą będę zamieniał na wektor embenddingowy na potrzeby szukania bazy wektorowej.";
    }
}
