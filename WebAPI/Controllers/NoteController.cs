using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedResources.Enums;
using SharedResources.Interfaces.Requests;
using SharedResources.Interfaces.Responses;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    public class NoteController : ControllerBase
    {
        [HttpPost]
        [Route("note/user")]
        public async Task<string> GetUserNotes(string? categories)
        {
            ITokenRequest request = await Core.GetFromBody<ITokenRequest>(Request.Body);
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidAccessToken).AsJson();
            IResponse response = Core.NoteManager.GetUserNotes(request, (String.IsNullOrEmpty(categories) ? null : (categories ?? "").Replace(" ", "").Split(',')));
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("note/browse")]
        public async Task<string> GetPersonalizedPublicNotes(string? categories, int? offset = null)
        {
            ITokenRequest? request = await Core.GetFromBody<ITokenRequest>(Request.Body);
            if (request == null || !request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.NoteManager.GetPublicNotes(request, (String.IsNullOrEmpty(categories) ? null : (categories ?? "").Replace(" ", "").Split(',')), offset);
            Response.StatusCode = ResolveStatusCode(response);
            Response.ContentLength = Encoding.UTF8.GetBytes(response.AsJson()).Length;
            return response.AsJson();
        }
        [HttpGet]
        [Route("note/browse")]
        public string GetPublicNotes(string? categories, int? offset = null)
        {
            IResponse response = Core.NoteManager.GetPublicNotes(null, (String.IsNullOrEmpty(categories) ? null : (categories ?? "").Replace(" ", "").Split(',')), offset);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("note/save")]
        public async Task<string> SaveUserNote()
        {
            IUserNoteRequest request = await Core.GetFromBody<IUserNoteRequest>(Request.Body);
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.NoteManager.SaveUserNote(request);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        [HttpPost]
        [Route("note/delete")]
        public async Task<string> DeleteUserNote()
        {
            IUserNoteRequest request = await Core.GetFromBody<IUserNoteRequest>(Request.Body);
            if (!request.IsValid()) return Core.CreateErrorResponse(Result.InvalidRequest).AsJson();
            IResponse response = Core.NoteManager.DeleteUserNote(request);
            Response.StatusCode = ResolveStatusCode(response);
            return response.AsJson();
        }

        private int ResolveStatusCode(IResponse response, bool post = false)
        {
            if (response is IErrorResponse)
            {
                IErrorResponse res = (IErrorResponse)response;
                return res.Error.ToLower().Contains("internal") ? 500 : 401;
            }
            else
                return post ? 201 : 200;
        }
    }
}