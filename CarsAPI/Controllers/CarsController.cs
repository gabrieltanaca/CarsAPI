using CarsAPI.Data.Repositories;
using CarsAPI.Models;
using CarsAPI.Models.InputModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CarsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private ICarsRepository _carsRepository;

        public CarsController(ICarsRepository carsRepository)
        {
            _carsRepository = carsRepository;
        }

        // GET: api/Cars
        [HttpGet]
        public IActionResult Get(
            [FromServices] ICarsRepository carsRepository,
            [FromQuery(Name = "Model")] string model,
            [FromQuery(Name = "Marca")] string marca,
            [FromQuery(Name = "Year")] int year,
            [FromQuery(Name = "Sold")] bool? sold,
            [FromQuery(Name = "ValueGreaterThan")] double valueGt,
            [FromQuery(Name = "ValueLessThan")] double valueLt,
            [FromQuery(Name = "CreationDateGreaterThan")] DateTime dateGt,
            [FromQuery(Name = "CreationDateLessThan")] DateTime dateLt,
            int? page
            )
        {
            // var cars = _carsRepository.GetCar();
            var cars = _carsRepository.Filter(model, marca, year, sold, valueGt, valueLt, dateGt, dateLt, page);

           return Ok(cars);
        }

        // GET api/Cars/{id}
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            /*
             if (string.IsNullOrEmpty(id))
                return NotFound();
            */

            var car = _carsRepository.GetCar(id);

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST api/Cars
        [HttpPost]
        public IActionResult Post([FromBody] CarInputModel newCar)
        {
            var car = new Car(newCar.Model, newCar.Marca, newCar.Year, newCar.SaleValue);

            _carsRepository.AddCar(car);

            return Created("", car);
        }

        // PUT api/Cars/{id}
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] CarInputModel updateCar)
        {
            var car = _carsRepository.GetCar(id);

            if (car == null)
                return NotFound();

            car.UpdateCarModel(updateCar.Model, updateCar.Marca, updateCar.Year, updateCar.SaleValue);

            _carsRepository.UpdateCar(id, car);

            return Ok(car);
        }

        // PUT api/Cars/Sold/{id}
        [HttpPut("{id}/Sold")]
        public IActionResult PutSold(
            string id, 
            [FromQuery(Name = "DiscountPercent")] double discountPercent,
            [FromQuery(Name = "DiscountValue")] double discountValue
            )
        {
            var car = _carsRepository.GetCar(id);
            double discount = discountValue;
            double soldValue = car.SaleValue - discountValue;

            if (car == null)
                return NotFound();

            if (car.Sold == true)
                return BadRequest("O Carro já foi vendido.");

            if (discountPercent >= 1 & discountValue >= 1)
                return BadRequest("Não pode haver dois tipos de desconto. Por favor escolha apenas um.");

            if (discountPercent > 100)
                return BadRequest("A porcentagem de desconto não pode ser maior que 100%");

            if (discountPercent != 0)
            {
                var convertPercent = (discountPercent / 100);

                discount = car.SaleValue * convertPercent;
                soldValue = car.SaleValue - discount;
            }

            car.SoldCar(soldValue, discount);

            _carsRepository.UpdateCar(id, car);

            return Ok(car);
        }

        // DELETE api/Cars/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var car = _carsRepository.GetCar(id);

            if (car == null)
                return NotFound();

            _carsRepository.DeleteCar(id);

            return NoContent();
        }

        [HttpGet("GetCarsMonth")]
        public IActionResult GetPeriod(
            [FromQuery(Name = "CreationDateGreaterThan")] DateTime dateGt,
            [FromQuery(Name = "CreationDateLessThan")] DateTime dateLt
            )
        {
            bool sold = true;

            var filterCount = _carsRepository.FilterPeriodo(sold, dateGt, dateLt);
            var filterSum = _carsRepository.FilterSum(dateGt, dateLt);

            string text = "Foram vendidos " + filterCount + " carros, havendo um total de vendas: R$" + filterSum;
            string datesTimes = ". No periodo de " + dateGt + " && " + dateLt;

            return Ok(text + datesTimes);
        }
    }
}
