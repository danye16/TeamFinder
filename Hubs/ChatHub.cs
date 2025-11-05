using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TeamFinder.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task EnviarMensaje(int remitenteId, int destinatarioId, string contenido)
        {
            // Aquí podrías guardar el mensaje en la base de datos
            // Por simplicidad, solo lo enviamos directamente

            // Envía el mensaje al destinatario específico
            await Clients.User(destinatarioId.ToString()).SendAsync("RecibirMensaje", remitenteId, contenido);

            // También envía una confirmación al remitente
            await Clients.Caller.SendAsync("MensajeEnviado", destinatarioId, contenido);
        }

        public async Task UnirseAlChat(int usuarioId)
        {
            // Agrega el usuario a un grupo basado en su ID
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Usuario_{usuarioId}");
        }

        public async Task SalirDelChat(int usuarioId)
        {
            // Remueve al usuario del grupo
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Usuario_{usuarioId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Este método se llama cuando un usuario se desconecta
            await base.OnDisconnectedAsync(exception);
        }
    }
}