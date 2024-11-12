using AccesoDatos.DAO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pruebas
{
    public class PruebaAmistadDao
    {
        private readonly AmistadDao amistadDao;

        public PruebaAmistadDao()
        {
            amistadDao = new AmistadDao();
        }

        [Fact]
        public void VerificarAmistad_CuandoExisteRelacion_RetornaTrue()
        {
            int idPlayerSender = 1;
            int idPlayerRequested = 2;

            var resultado = amistadDao.VerificarAmistad(idPlayerSender, idPlayerRequested);

            Assert.True(resultado);
        }

        [Fact]
        public void VerificarAmistad_CuandoNoExisteRelacion_RetornaFalse()
        {
            int idPlayerSender = 1;
            int idPlayerRequested = 3;

            var resultado = amistadDao.VerificarAmistad(idPlayerSender, idPlayerRequested);

            Assert.False(resultado);
        }

        [Fact]
        public void VerificarAmistad_ExcepcionEntidad_LanzaExcepcion()
        {
            int idPlayerSender = 1;
            int idPlayerRequested = 2;

            Assert.Throws<EntityException>(() => amistadDao.VerificarAmistad(idPlayerSender, idPlayerRequested));
        }

        [Fact]
        public void AgregarSolicitudAmistad_CuandoEsValida_RetornaFilasAfectadas()
        {
            int idPlayerSender = 1;
            int idPlayerRequested = 2;

            var filasAfectadas = amistadDao.AgregarSolicitudAmistad(idPlayerSender, idPlayerRequested);

            Assert.True(filasAfectadas > 0);
        }

        [Fact]
        public void AgregarSolicitudAmistad_ExcepcionSql_LanzaExcepcion()
        {
            int idPlayerSender = 1;
            int idPlayerRequested = 2;

            Assert.Throws<SqlException>(() => amistadDao.AgregarSolicitudAmistad(idPlayerSender, idPlayerRequested));
        }

        [Fact]
        public void EsAmigo_CuandoEsAmigo_RetornaTrue()
        {
            int idPlayer = 1;
            int idPlayerFriend = 2;

            var esAmigo = amistadDao.EsAmigo(idPlayer, idPlayerFriend);

            Assert.True(esAmigo);
        }

        [Fact]
        public void EsAmigo_CuandoNoEsAmigo_RetornaFalse()
        {
            int idPlayer = 1;
            int idPlayerFriend = 3;

            var esAmigo = amistadDao.EsAmigo(idPlayer, idPlayerFriend);

            Assert.False(esAmigo);
        }

        [Fact]
        public void ObtenerIdJugadoresQueSolicitaronAmistad_Exitoso()
        {
            int idPlayer = 1;

            var resultado = amistadDao.ObtenerIdJugadorSolicitantesAmistad(idPlayer);

            Assert.NotEmpty(resultado);
        }

        [Fact]
        public void ActualizarSolicitudAmistadAceptada_CuandoExiste_RetornaFilasAfectadas()
        {
            int idCurrentPlayer = 1;
            int idPlayerAccepted = 2;

            var filasAfectadas = amistadDao.ActualizarSolicitudAmistad_Aceptada(idCurrentPlayer, idPlayerAccepted);

            Assert.True(filasAfectadas > 0);
        }

        [Fact]
        public void ActualizarSolicitudAmistadAceptada_CuandoNoExiste_RetornaCero()
        {
            int idCurrentPlayer = 1;
            int idPlayerAccepted = 3;

            var filasAfectadas = amistadDao.ActualizarSolicitudAmistad_Aceptada(idCurrentPlayer, idPlayerAccepted);

            Assert.Equal(0, filasAfectadas);
        }

        [Fact]
        public void EliminarSolicitudAmistad_CuandoExiste_RetornaFilasAfectadas()
        {
            int idCurrentPlayer = 1;
            int idPlayerRejected = 2;

            var filasAfectadas = amistadDao.BorrarSolicitudAmistad(idCurrentPlayer, idPlayerRejected);

            Assert.True(filasAfectadas > 0);
        }

        [Fact]
        public void EliminarAmistad_CuandoExiste_RetornaFilasAfectadas()
        {
            int idCurrentPlayer = 1;
            int idPlayerFriend = 2;

            var filasAfectadas = amistadDao.BorrarAmistad(idCurrentPlayer, idPlayerFriend);

            Assert.True(filasAfectadas > 0);
        }

        [Fact]
        public void ObtenerAmigos_CuandoExistenAmigos_RetornaLista()
        {
            int idPlayer = 1;

            var amigos = amistadDao.ObtenerAmigos(idPlayer);

            Assert.NotEmpty(amigos);
        }

        [Fact]
        public void ObtenerAmigos_ExcepcionEntidad_LanzaExcepcion()
        {
            int idPlayer = 1;

            Assert.Throws<EntityException>(() => amistadDao.ObtenerAmigos(idPlayer));
        }
    }
}
