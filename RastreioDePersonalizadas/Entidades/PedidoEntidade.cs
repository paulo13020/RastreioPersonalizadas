using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RastreioDePersonalizadas.Entidades
{
    [Table("Pedidos")]
    public class PedidoEntidade
    {
        [Key]
        public int ID { get; set; }
        public int PDV_NUMERO  { get; set; }
        public string? REP_NOME { get; set; }
        public string? PDV_OBS1 { get; set; }
        public string? PDV_OBS2 { get; set; }
        public string? PDV_OBS3 { get; set; }
        public string? PDV_OBS4 { get; set; }
        public string Modelo { get; set; }
        public int? QUANTIDADE_YALE { get; set; }
        public int? QUANTIDADE_TETRA { get; set; }
        public string? Usuario { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? Data { get; set; }
        public int Status_ID { get; set; }
        public string Filial{ get; set; }
        public string Responsavel { get; set; }
        public int Nota_Fiscal { get; set; }
        public int Volume { get; set; }
        public int Romaneio { get; set; }
        [NotMapped]
        [JsonIgnore]
        public List<ProdutosPedidoEntidade> Itens { get; set; } = new List<ProdutosPedidoEntidade>();
    }   
}
