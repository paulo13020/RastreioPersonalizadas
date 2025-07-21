using System.ComponentModel.DataAnnotations.Schema;

namespace RastreioDePersonalizadas.Entidades
{
    [Table("Status")]
    public class StatusEntidade
    {
        public int ID { get; set; }
        public string Status_Nome { get; set; }    
    }
}
