using GiocoMonete_Dadi;

CSacchetto sacchetto = new CSacchetto();

while (sacchetto.ContinuaGioco()) 
{
    Console.WriteLine(sacchetto.LanciaRandom());
}