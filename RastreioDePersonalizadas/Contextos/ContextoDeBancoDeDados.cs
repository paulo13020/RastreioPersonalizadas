using Microsoft.EntityFrameworkCore;
using RastreioDePersonalizadas.Entidades;

namespace RastreioDePersonalizadas.Contextos
{
    public class ContextoDeBancoDeDados : DbContext
    {
        #region Construtores
        public ContextoDeBancoDeDados(DbContextOptions<ContextoDeBancoDeDados> options ) : base(options) { }

        public ContextoDeBancoDeDados() { }
        #endregion

        public DbSet<StatusEntidade> Status { get; set; }
        public DbSet<PedidoEntidade> Pedidos { get; set; }
        public DbSet<StatusDoPedidoEntidade> StatusDoPedido { get; set; }
        public DbSet<ProdutosPedidoEntidade> ProdutosPedido { get; set; }
        public DbSet<NotaFiscalPedidoEntidade> NotaFiscalPedido { get; set; }
        public DbSet<LogEntidade> Logs { get; set; }

        public string MontarStringDeConexao(string LocalDeConexao)
        {
            var stringDeConexao = "";

            switch (LocalDeConexao)
            {
                case "saojose":
                    stringDeConexao = "User=SYSDBA; Password=masterkey; Database=192.168.10.37:C:\\Microsys\\SJC\\MsysIndustrial\\dados\\MSYSDADOS.FDB; DataSource=192.168.10.37;Dialect=3;Charset=UTF8;Pooling=true";
                    break;

                case "sapucai":
                    stringDeConexao = "User=SYSDBA; Password=masterkey; Database=192.168.10.37:C:\\Microsys\\MG\\MsysIndustrial\\Dados\\MSYSDADOS.FDB; DataSource=192.168.10.37;Dialect=3;Charset=UTF8;Pooling=true";
                    break;
            }
            return stringDeConexao;
        }
    }
}