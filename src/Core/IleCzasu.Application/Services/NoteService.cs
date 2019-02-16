using IleCzasu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IleCzasu.Application.Interfaces;
using IleCzasu.Data;
using Microsoft.EntityFrameworkCore;

namespace IleCzasu.Application.Services
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;

        public NoteService(ApplicationDbContext context, IPublicEventService publicEventService)
        {
            _context = context;
           
        }
        public async Task AddNote(Note note)
        {
            if (note != null)
                await _context.Notes.AddAsync(note);
        }

        public async Task DeleteNote(int noteId)
        {
            var model = await GetNoteById(noteId);
            if (model != null)
                _context.Notes.Remove(model);
        }

        public async Task<Note> GetNoteById(int noteId)
        {
            return await _context.Notes.SingleOrDefaultAsync(x => x.NoteId == noteId);
        }
    }
}
