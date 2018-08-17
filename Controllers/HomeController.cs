﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConnectedNotes.Models;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ConnectedNotes.Controllers
{
    public class HomeController : Controller
    {

        string notesName = "notes";
        public IActionResult Index()
        {
            return View();
        }

        private NotesRepo retrieveNotesRepo()
        {
            byte[] value; 
            var retrieved = HttpContext.Session.TryGetValue(notesName, out value);
            BinaryFormatter formatter = new BinaryFormatter();
            
            if(retrieved && value != null)
            {
                var ms = new MemoryStream(value);
                return formatter.Deserialize(ms) as NotesRepo;
            } 
            return  new NotesRepo(); 
        }

        private void saveNotes(NotesRepo repo)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, repo);
            HttpContext.Session.Set(notesName, ms.ToArray());
            ms.Close();
        }

        public JsonResult RetrieveAllNotes()
        {
            var repo = retrieveNotesRepo();
            return new JsonResult(repo);
        }

        public JsonResult CreateNote()
        {
            var repo = retrieveNotesRepo();
            List<Note> notes = repo.Notes;
            var count = repo.FreeIndex;
            var date = DateTime.Now;
            var toAdd = new Note
                {
                    Id = count,
                    Text = "",
                    CreatedOn = date,
                    UpdatedOn = date
                };
            notes.Add(
                toAdd
            );
            repo.FreeIndex++;
            saveNotes(repo);
            return new JsonResult(toAdd);
        }

        public JsonResult ConnectNotes(Note from, Note to, string label)
        {
            var repo = retrieveNotesRepo();
            List<Note> notes = repo.Notes;
            var foundTo = notes.FirstOrDefault(x => x.Id == to.Id);
            if(foundTo != null)
            {
                var foundFrom = notes.FirstOrDefault(x => x.Id == from.Id);
                
                if(foundFrom != null)
                {
                    var toAdd = new Connection 
                        {
                            Source = from,
                            Destination = to,
                            Label = label,
                            Id = repo.FreeIndex++
                        };

                    repo.Connections.Add(toAdd);
                    saveNotes(repo);
                    return new JsonResult(toAdd);
                }
            }
            throw new Exception("Not found prerequisites.");
        }

        public JsonResult UpdateNote(Note note)
        {
            var repo = retrieveNotesRepo();
            List<Note> notes = repo.Notes;
            var foundNote = notes.FirstOrDefault(x => x.Id == note.Id);
            foundNote.Text = note.Text;
            saveNotes(repo);
            return new JsonResult(foundNote);
        }

        public JsonResult UpdateConnection(Connection connection)
        {
            var repo = retrieveNotesRepo();
            var found = repo.Connections.FirstOrDefault(c => c.Id == connection.Id);
            found.Label = connection.Label;
            saveNotes(repo);
            return new JsonResult(found);
        }

        public JsonResult RemoveNote(Note note)
        {
            var repo = retrieveNotesRepo();
            var found = repo.Notes.FirstOrDefault(n => n.Id == note.Id);
            if(found != null)
            {
                repo.Notes = repo.Notes.Where(n => n.Id != note.Id).ToList();
                repo.Connections = repo.Connections.Where(c => !(c.Source.Id == note.Id || c.Destination.Id == note.Id)).ToList();
            }
            saveNotes(repo);
            return new JsonResult(true);
        }

        public JsonResult RemoveConnection(long id)
        {
            var repo = retrieveNotesRepo();
            var found = repo.Connections.FirstOrDefault(c => c.Id == id);
            if(found != null)
            {
                repo.Connections = repo.Connections.Where(c => c.Id != id).ToList();
            }
            saveNotes(repo);
            return new JsonResult(true);
        }
    }
}
