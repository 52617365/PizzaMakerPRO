namespace NameConvention
{

    // Kommentointi

    /*
    Monen linjan kommentointi näin

    */

    // NameConvention luokassa käydään luokan rakenne läpi

    class NameConvention
    {
        // Fieldit ensin
        private string exampleField; //camelCase

        // Sitten propertyt

        public string ExampleProperty
        { get; private set; }    // PascalCase

        /*
        
        Tämä on shorthand tapa tehdä tämä asia:

        private string exampleProperty;

        public string ExampleProperty
        {
            get { return exampleField; }
            private set { exampleField = value; }
        }

        Shorthand mielestäni siksi, koska shorthand tapa on luettavampi

        */


        // Sitten constructor (Rakentaja)

        // ctor + tab tab vinkiksi constructorin nopeaan tekemiseen visual studio codessa / visual studiossa =)

        public NameConvention(string exampleField)
        {
            ExampleField = exampleField;    
        }

        // Täällä vasta methodit
        // Access modifier methodin tarkoituksen mukaan

        public string ExampleMethod(string parameter)   // PascalCase ja parameteri camelCase niinkuin kaikki variablet ja fieldit
        {

            if (parameter == "Esimerkki")
            {
                return parameter;
            }
            return "Ei ollut esimerkki";
            // ( Sama kuin else mutta mielestäni luettavampi, mielipiteitä otetaan vastaan )

        }

    }

    // Tässä luokassa käydään läpi conditional statementit (if, switch case etc.)
    class ConditionalStatements
    {
        
        private string ifStatements = "esimerkki";

        // If statement
        public void IfStatements()
        {

            if (ifStatements == "esimerkki")
            {
            // Do something
            }
            // Else do something

        }

        public int SwitchStatement()
        {
            int esimerkkiNumero = 3;  // Variablet camelCase

            // Mielestäni käytetään vain silloin kun ei tarvitse tehdä laskutoimituksia

            switch(esimerkkiNumero)   
            {
                case 1: return 1; break;       // Jos one lineri niinkuin tässä niin laitetaan samalle linjalle 
                case 2: return 2; break;
                case 3: return 3; break;
                default: return 0; break;
            }
        }

        public interface IName      // Interfacen nimeäminen niinkuin C# ohjeistaa eli iso I kirjain ennen interfacen nimeämistä
        {
            void ExampleMethod();
            string Name { get; set; }
            int Number { get; private set; }
        }

        abstract class AbstractClass
        {
            public int NormalMethod(int par1, int par2); // Normal method
            public abstract int AbstractMethod(int par1, int par2); // Abstract method (made to be overridden in derived / child class)
        }


    }
}