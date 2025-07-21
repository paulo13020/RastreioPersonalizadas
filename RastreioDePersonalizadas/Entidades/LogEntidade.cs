using System.ComponentModel.DataAnnotations.Schema;

namespace RastreioDePersonalizadas.Entidades
{
    [Table("LOG")]
    public class LogEntidade
    {
        public int ID { get; set; }
        public string Usuario { get; set; }
        public string Detalhe { get; set; }
        public DateTime Data { get; set; }
    }
}
