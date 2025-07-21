using System.ComponentModel.DataAnnotations;

namespace RastreioDePersonalizadas.ObjetoDeTransferencia
{
    public class PedidoOTE
    {
        [Key]
        public int ID { get; set; }
        public int? PDV_NUMERO { get; set; }
        public string? REP_NOME { get; set; }
        public string? PDV_OBS1 { get; set; }
        public string? PDV_OBS2 { get; set; }
        public string? PDV_OBS3 { get; set; }
        public string? PDV_OBS4 { get; set; }
        public string? Modelo { get; set; }
        public int? QUANTIDADE_YALE { get; set; }
        public int? QUANTIDADE_TETRA { get; set; }
        public string? Usuario { get; set; }
        public DateTime? Data { get; set; }
        public string? Status { get; set; }
        public string? Responsavel { get; set; }
        public string? Filial { get; set; }
        public int Nota_Fiscal { get; set; }
        public int Volume { get; set; }
        public int Romaneio { get; set; }
    }
}
