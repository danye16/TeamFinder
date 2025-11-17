using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TeamFinder.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task UnirseConversacion(int usuarioId1, int usuarioId2)
        {
            // Crear un nombre de grupo único para la conversación entre dos usuarios
            string grupo = ObtenerNombreGrupo(usuarioId1, usuarioId2);
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);

            await Clients.Group(grupo).SendAsync("UsuarioConectado", Context.ConnectionId);
        }

        public async Task SalirConversacion(int usuarioId1, int usuarioId2)
        {
            string grupo = ObtenerNombreGrupo(usuarioId1, usuarioId2);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);

            await Clients.Group(grupo).SendAsync("UsuarioDesconectado", Context.ConnectionId);
        }

        public async Task UnirseGrupoUsuario(int usuarioId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Usuario_{usuarioId}");
        }

        public async Task SalirGrupoUsuario(int usuarioId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Usuario_{usuarioId}");
        }

        public async Task NotificarEscribiendo(int remitenteId, int destinatarioId, bool escribiendo)
        {
            string grupo = ObtenerNombreGrupo(remitenteId, destinatarioId);
            await Clients.OthersInGroup(grupo).SendAsync("UsuarioEscribiendo", remitenteId, escribiendo);
        }

        public async Task NotificarMensajeLeido(int mensajeId, int remitenteId, int destinatarioId)
        {
            string grupo = ObtenerNombreGrupo(remitenteId, destinatarioId);
            await Clients.Group(grupo).SendAsync("MensajeLeido", mensajeId);
        }

        // Método auxiliar para generar un nombre de grupo único para una conversación entre dos usuarios
        private string ObtenerNombreGrupo(int usuarioId1, int usuarioId2)
        {
            // Ordenamos los IDs para que el grupo sea el mismo sin importar el orden
            return usuarioId1 < usuarioId2 ?
                $"Conversacion_{usuarioId1}_{usuarioId2}" :
                $"Conversacion_{usuarioId2}_{usuarioId1}";
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}