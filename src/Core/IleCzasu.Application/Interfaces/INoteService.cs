using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IleCzasu.Application.Interfaces
{
    public interface INoteService
    {
        Task<Note> GetNoteById(int noteId);
        Task<List<Note>> GetUserNotes(string userId, string date = "");
        Task AddNote(Note note);
        Task DeleteNote(int noteId);
    }
}
