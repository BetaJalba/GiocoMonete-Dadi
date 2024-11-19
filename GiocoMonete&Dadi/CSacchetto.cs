using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GiocoMonete_Dadi
{
    public class CDado : ILanciabile 
    {
        static int step = 6;
        static int count = 1;
        string nome;

        public CDado() 
        {
            nome = "Dado" + count;
            count++;
        }

        public int Lancia() 
        {
            Random random = new Random();
            return random.Next(step) + 1;
        }

        public string Nome() 
        {
            return nome;
        }
    }

    public class CMonetaConMemoria : ILanciabile
    {
        static int step = 2; // 0 - croce; 1 - testa
        static int count = 1;
        string nome;

        List<int> lanci;

        public EventHandler? OnDoppiaTesta;
        public EventHandler? OnTriplaCroce;

        public CMonetaConMemoria() 
        {
            nome = "Moneta" + count;
            count++;
            lanci = new List<int>(3);
        }

        public int Lancia() 
        {
            Random random = new Random();
            if (lanci.Count > 2)
                lanci.RemoveAt(0);
            lanci.Add(random.Next(step));
            ControllaEventi();
            return lanci.Last();
        }

        private void ControllaEventi() 
        {
            if (lanci.Where(lancio => lancio == 0).ToArray().Length == 2)
                OnDoppiaTesta(this, EventArgs.Empty);
            else if (lanci.Where(lancio => lancio == 1).ToArray().Length == 3)
                OnTriplaCroce(this, EventArgs.Empty);
        }

        public void ResettaMemoria() 
        {
            lanci = new List<int>(3);
        }

        public string Nome() 
        {
            return nome;
        }
    }

    public class CSacchetto
    {
        bool canPlay;
        int oldMoneta;

        List<CDado> dadi = new List<CDado>(5);
        List<CMonetaConMemoria> monete = new List<CMonetaConMemoria>(10);

        public CSacchetto() 
        {
            canPlay = true;
            oldMoneta = -1;

            for (int i = 0; i < 10; i++) 
            {
                if (i < 5)
                    dadi.Add(new CDado());

                monete.Add(new CMonetaConMemoria());
                monete.Last().OnDoppiaTesta += (sender, e) =>
                {
                    if (monete.IndexOf(sender as CMonetaConMemoria) < oldMoneta)
                        oldMoneta--;
                    else if (monete.IndexOf(sender as CMonetaConMemoria) == oldMoneta)
                        oldMoneta = -1;

                    monete.Remove(sender as CMonetaConMemoria);

                    if (monete.Count == 0)
                        canPlay = false;
                };
                monete.Last().OnTriplaCroce += (sender, e) => canPlay = false;
            }
        }

        public string LanciaRandom() 
        {
            Random random = new Random();
            string r;
            int last;

            if (random.Next(2) == 0)
            {
                last = random.Next(monete.Count);
                if (last != oldMoneta && oldMoneta != -1)
                    monete[oldMoneta].ResettaMemoria();
                oldMoneta = last;
                r = monete[last].Nome() + " ha ottenuto " + monete[last].Lancia();
            } else 
            {
                last = random.Next(dadi.Count);
                if (oldMoneta != -1)
                    monete[oldMoneta].ResettaMemoria();
                r = dadi[last].Nome() + " ha ottenuto " + dadi[last].Lancia();
            }

            return r;
        }

        public bool ContinuaGioco() 
        {
            return canPlay;
        }
    }
}
