using System.ComponentModel.DataAnnotations.Schema;

namespace RastreioDePersonalizadas.Entidades
{
    [Table("Pedidos_Rastreio")]
    public class StatusDoPedidoEntidade
    {
        public int ID { get; set; }
        public int Status_ID { get; set; }
        public int Pedido_ID { get; set; }
        public bool? Status_1 { get; set; }
        public bool? Status_2 { get; set; }
        public bool? Status_3 { get; set; }
        public bool? Status_4 { get; set; }
        public bool? Status_5 { get; set; }
        public bool? Status_6 { get; set; }
        public bool? Status_7 { get; set; }
        public bool? Status_8 { get; set; }
        public bool? Status_9 { get; set; }
        public bool? Status_10 { get; set; }
        public bool? Status_11 { get; set; }
        public bool? Status_12 { get; set; }
        public string? Usu_1 { get; set; }
        public string? Usu_2 { get; set; }
        public string? Usu_3 { get; set; }
        public string? Usu_4 { get; set; }
        public string? Usu_5 { get; set; }
        public string? Usu_6 { get; set; }
        public string? Usu_7 { get; set; }
        public string? Usu_8 { get; set; }
        public string? Usu_9 { get; set; }
        public string? Usu_10 { get; set; }
        public string? Usu_11 { get; set; }
        public string? Usu_12 { get; set; }
        public DateTime? DataStatus_1 { get; set; }
        public DateTime? DataStatus_2 { get; set; }
        public DateTime? DataStatus_3 { get; set; }
        public DateTime? DataStatus_4 { get; set; }
        public DateTime? DataStatus_5 { get; set; }
        public DateTime? DataStatus_6 { get; set; }
        public DateTime? DataStatus_7 { get; set; }
        public DateTime? DataStatus_8 { get; set; }
        public DateTime? DataStatus_9 { get; set; }
        public DateTime? DataStatus_10 { get; set; }
        public DateTime? DataStatus_11 { get; set; }
        public DateTime? DataStatus_12 { get; set; }
    }
}
