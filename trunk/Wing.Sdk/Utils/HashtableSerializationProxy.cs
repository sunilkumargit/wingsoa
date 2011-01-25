using System;
using System.Collections;

namespace Wing.Utils
{
    public class HashtableSerializationProxy : ICollection
    {
        //a tabela hash que que será serializada
        private Hashtable _hashTable;
        //um enumerator para percorrer o hash table.
        private IDictionaryEnumerator _enumerator;
        private int _position = -1;

        public HashtableSerializationProxy(Hashtable hashTable)
        {
            _hashTable = hashTable;
            _position = -1;
        }

        //SERIALIZACAO: A classe XMLSerializer irá usar este metodo para pegar um item para serializar no xml.
        public DictionaryEntry this[int index]
        {
            get
            {
                if (_enumerator == null)  // lazy
                    _enumerator = _hashTable.GetEnumerator();

                // Acessar um item cuja a posicao seja anterior a ultima recebida é anormal,
                // pois o XMLSerializaer irá chamar este metodo varias vezes, passando o indice
                // de forma crescente. 
                // Mas, caso isso aconteça, então é necessario chamar o Reset() do enumerator
                // pois ele trabalha 'forward-only'.
                if (index < _position)
                {
                    _enumerator.Reset();
                    _position = -1;
                }

                while (_position < index)
                {
                    _enumerator.MoveNext();
                    _position++;
                }
                return _enumerator.Entry;
            }
        }

        //DESERIALIZACAO: O XmlSerializar irá chamar este metodo para adicionar um item que foi deserializado.
        public void Add(DictionaryEntry de)
        {
            _hashTable[de.Key] = de.Value;
        }

        // O resto dos metodos de ICollection eu repasso para os metodos do Hashtable.
        public int Count { get { return _hashTable.Count; } }
        public bool IsSynchronized { get { return _hashTable.IsSynchronized; } }
        public object SyncRoot { get { return _hashTable.SyncRoot; } }
        public void CopyTo(Array array, int index) { _hashTable.CopyTo(array, index); }
        public IEnumerator GetEnumerator() { return _hashTable.GetEnumerator(); }
    }
}