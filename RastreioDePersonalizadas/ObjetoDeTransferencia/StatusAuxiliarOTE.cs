namespace RastreioDePersonalizadas.ObjetoDeTransferencia
{
    public class StatusAuxiliarOTE
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public bool Marcado { get; set; }
        public DateTime? StatusData { get; set; }
        public bool IsLast { get; set; }
        public DateTime? Data { get; set; }
    }
}
