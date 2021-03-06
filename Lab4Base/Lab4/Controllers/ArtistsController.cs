﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
// new...
using Lab4.ServiceLayer;

namespace Lab4.Controllers
{
    public class ArtistsController : ApiController
    {
        // Reference to the worker object
        private Worker w;
        private Manager m;

        public ArtistsController()
        {
            w = new Worker();
            m = new Manager(w);
        }

        // IHttpActionResult return types for GET and POST (and PUT methods that do edits)

        // GET: api/Artists
        public IHttpActionResult Get()
        {
            return Ok(w.Artists.GetAll());
        }

        // GET: api/Artists/WithMembers
        [Route("api/artists/withmembers")]
        public IHttpActionResult GetWithMembers()
        {
            return Ok(w.Artists.GetAllWithMembers());
        }

        // GET: api/Artists/5
        public IHttpActionResult Get(int? id)
        {
            // Fetch the object
            var fetchedObject = w.Artists.GetById(id.GetValueOrDefault());

            return (fetchedObject == null) ? NotFound() : (IHttpActionResult)Ok(fetchedObject);
        }

        // GET: api/Artists/5/WithMembers
        [Route("api/artists/{id}/withmembers")]
        public IHttpActionResult GetWithMembers(int? id)
        {
            // Fetch the object
            var fetchedObject = w.Artists.GetByIdWithMembers(id.GetValueOrDefault());

            return (fetchedObject == null) ? NotFound() : (IHttpActionResult)Ok(fetchedObject);
        }

        // POST: api/Artists
        public IHttpActionResult Post([FromBody]ArtistAdd newItem)
        {
            // Ensure that the URI is clean (and does not have an id parameter)
            if (Request.GetRouteData().Values["id"] != null) { return BadRequest("Invalid request URI"); }

            // Ensure that a "newItem" is in the entity body
            if (newItem == null) { return BadRequest("Must send an entity body with the request"); }

            // Ensure that we can use the incoming data
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            // Attempt to add the new object
            var addedItem = w.Artists.Add(newItem);

            // Continue?
            if (addedItem == null) { return BadRequest("Cannot add the object"); }

            // HTTP 201 with the new object in the entity body
            // Notice how to create the URI for the Location header
            var uri = Url.Link("DefaultApi", new { id = addedItem.Id });
            return Created<ArtistBase>(uri, addedItem);
        }

        // This code uses the "command pattern" described in the September 24 notes

        // PUT: api/Artists/5/SetGroup
        [Route("api/artists/{id}/setgroup")]
        public void PutSetGroup(int id, [FromBody]ArtistMemberGroup item)
        {
            // Ensure that an "editedItem" is in the entity body
            if (item == null) { return; }

            // This use case is from the perspective of the "member" artist
            // Ensure that the id value in the URI matches the id value in the entity body
            if (id != item.Member) { return; }

            // Ensure that we can use the incoming data
            if (ModelState.IsValid) { w.Artists.SetMemberGroup(item); }
        }

        // This code uses the "command pattern" described in the September 24 notes

        // PUT: api/Artists/5/AddMember
        [Route("api/artists/{id}/addmember")]
        public void PutAddMember(int id, [FromBody]ArtistMemberGroup item)
        {
            // Ensure that an "editedItem" is in the entity body
            if (item == null) { return; }

            // This use case is from the perspective of the "group" artist
            // Ensure that the id value in the URI matches the id value in the entity body
            if (id != item.Group) { return; }

            // Ensure that we can use the incoming data
            if (ModelState.IsValid) { w.Artists.SetMemberGroup(item); }
        }

        /*
        // PUT: api/Artists/5
        public void Put(int id, [FromBody]string value)
        {
        }
        */

        // DELETE: api/Artists/5
        public void Delete(int id)
        {
            // In a controller 'Delete' method, a void return type will
            // automatically generate a HTTP 204 "No content" response
            w.Artists.DeleteExisting(id);
        }

    }

}
