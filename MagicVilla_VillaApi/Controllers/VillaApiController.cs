using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace MagicVilla_VillaApi.Controllers
{
    [ApiController, Route("api/VillaApi")]
    public class VillaApiController : ControllerBase
    {

        private readonly ApplicationDbContext _dbContext;
        public VillaApiController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {

            return Ok(await _dbContext.villas.ToListAsync());
        }

        [HttpGet("Id", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {

                return BadRequest();
            }
            var villa = await _dbContext.villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa != null)
            {

                return Ok(villa);
            }
            return NotFound();
        }
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaDto villa)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (_dbContext.villas.FirstOrDefaultAsync(v => v.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists!");
                return BadRequest(ModelState);
            }
            if (villa == null)
            {
                return BadRequest(villa);
            }
            if (villa.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa model = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };
            _dbContext.villas.Add(model);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetVilla", new VillaDto { Id = villa.Id }, villa);
        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("Id", Name = "DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _dbContext.villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _dbContext.villas.Remove(villa);
            return NoContent();


        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("Id", Name = "UpdateVilla")]

        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villa)
        {
            if (villa == null || id != villa.Id)
            {
                return BadRequest();
            }
            //var theVilla = _dbContext.villas.FirstOrDefault(v => v.Id == id);
            Villa model = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft

            };
            _dbContext.Update(model);
            return NoContent();


        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        [HttpPatch("Id", Name = "UpdatePartialVilla")]

        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchVilla)
        {
            if (id == 0 || patchVilla == null)
            {
                return BadRequest();
            }
            var theVilla = _dbContext.villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (theVilla == null)
            {
                return BadRequest();
            }
            VillaDto model = new()
            {
                Amenity = theVilla.Amenity,
                Details = theVilla.Details,
                Id = theVilla.Id,
                ImageUrl = theVilla.ImageUrl,
                Name = theVilla.Name,
                Occupancy = theVilla.Occupancy,
                Rate = theVilla.Rate,
                Sqft = theVilla.Sqft

            };
            patchVilla.ApplyTo(model, ModelState);
            Villa modelVilla = new()
            {
                Amenity = model.Amenity,
                Details = model.Details,
                Id = model.Id,
                ImageUrl = model.ImageUrl,
                Name = model.Name,
                Occupancy = model.Occupancy,
                Rate = model.Rate,
                Sqft = model.Sqft

            };
            _dbContext.villas.Update(modelVilla);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}