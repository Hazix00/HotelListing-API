using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using HotelListingAPI.HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        // [Authorize]
        // [HttpGet]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // public async Task<ActionResult<IList<CountryDTO>>> GetCountries()
        // {
        //     try
        //     {
        //         var countries = await _unitOfWork.Countries.GetAll(null,null, new List<string> { "Hotels" });
        //         var results = _mapper.Map<IList<CountryDTO>>(countries);
        //         return Ok(results);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetCountries)}");
        //         return StatusCode(500, "Internal Server Error. Please Try Again Later.");
        //     }
        // }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IPagedList<Country>>> GetCountries([FromQuery] RequestParams requestParams)
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAll(requestParams);

                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetCountries)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CountryDTO>> GetCountry(int id)
        {
            try
            {
                var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels" });
                var result = _mapper.Map<CountryDTO>(country);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetCountry)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CountryDTO>> CreateCountry([FromBody] CreateCountryDTO createCountryDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST Attemt in {nameof(CreateCountry)}");
                return BadRequest(ModelState);
            }
            try
            {
                var country = _mapper.Map<Country>(createCountryDTO);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.Save();

                return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(CreateCountry)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO updateCountryDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid PUT Attemt in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }
            try
            {
                var country = await _unitOfWork.Countries.Get(c => c.Id == id);
                if (country == null)
                {
                    return NotFound();
                }
                country.Hotels = new List<Hotel>();
                _mapper.Map(updateCountryDTO, country);
                _mapper.Map(updateCountryDTO.Hotels, country.Hotels);
                // updateCountryDTO.Hotels.ToList().ForEach(hotel => {
                //     country.Hotels.Add(_mapper.Map<Hotel>(hotel));
                // });
                _unitOfWork.Countries.Update(country);

                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateCountry)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CountryDTO>> DeleteCountry(int id)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid DELETE Attemt in {nameof(DeleteCountry)}");
                return BadRequest(ModelState);
            }
            try
            {
                var country = await _unitOfWork.Countries.Get(c => c.Id == id);
                if (country == null)
                {
                    return NotFound();
                }
                
                await _unitOfWork.Countries.Delete(id);

                await _unitOfWork.Save();

                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(DeleteCountry)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }
    }
}
