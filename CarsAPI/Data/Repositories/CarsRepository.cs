using CarsAPI.Data.Configurations;
using CarsAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarsAPI.Data.Repositories
{
    public class CarsRepository : ICarsRepository
    {   
        private readonly IMongoCollection<Car> _cars;
        private readonly FilterDefinitionBuilder<Car> filterBuilder = Builders<Car>.Filter;
        
        public CarsRepository(IDatabaseConfig databaseConfig)
        {
            var client = new MongoClient(databaseConfig.ConnectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);

            _cars = database.GetCollection<Car>("cars");
        }

        public void AddCar(Car car)
        {
            _cars.InsertOne(car);
        }

        public void UpdateCar(string id, Car updatedCar)
        {
            _cars.ReplaceOne(cars => cars.Id == id, updatedCar);
        }

        public IEnumerable<Car> GetCar()
        {
            return _cars.Find(new BsonDocument()).ToList();
        }

        public Car GetCar(string id)
        {
            return _cars.Find(car => car.Id == id).FirstOrDefault();
        }

        public void DeleteCar(string id)
        {
            _cars.DeleteOne(car => car.Id == id);
        }

        public object Filter(string model, string marca, int year, bool? sold, double valueGt, double valueLt, DateTime dateGt, DateTime dateLt, int? page)
        {
            var builder = Builders<Car>.Filter;

            FilterDefinition<Car> filters = builder.Empty;

            if (!string.IsNullOrEmpty(model))
                filters &= builder.Regex(m => m.Model, new BsonRegularExpression(model, "i"));

            if (!string.IsNullOrEmpty(marca))
                filters &= builder.Regex(m => m.Marca, new BsonRegularExpression(marca, "i"));

            if (year != 0)
                filters &= builder.Eq(f => f.Year, year);

            if(sold != null)
                filters &= builder.Eq(f => f.Sold, sold);

            if (valueGt != 0)
                filters &= builder.Gt(f => f.SaleValue, valueGt);

            if (valueLt != 0)
                filters &= builder.Lt(f => f.SaleValue, valueLt);

            if (dateGt != default)
                filters &= builder.Gt(f => f.CreationDate, dateGt);

            if (dateLt != default)
                filters &= builder.Lt(f => f.CreationDate, dateLt);

            var find = _cars.Find(filters);
            int pageValue = page.GetValueOrDefault(1) == 0 ? 1 : page.GetValueOrDefault(1);
            int pageSize = 5;

            return find.Skip((pageValue - 1) * pageSize).Limit(pageSize).ToList();
        }

        public object FilterPeriodo(bool sold, DateTime dateGt, DateTime dateLt)
        {
            var filters = filterBuilder.Empty;

            var filterCount = filterBuilder.Where(s => s.Sold == sold) 
                & filterBuilder.Lte(f => f.CreationDate, dateLt) 
                & filterBuilder.Gte(f => f.CreationDate, dateGt);
            
            var cars = _cars.Find(filterCount).ToList();

            return _cars.Find(filterCount).CountDocuments();
        }

        public object FilterSum(DateTime dateGt, DateTime dateLt)
        {

            var filters = filterBuilder.Empty;

            var filterCount = filterBuilder.Where(s => s.Sold == true)
                & filterBuilder.Lte(f => f.CreationDate, dateLt)
                & filterBuilder.Gte(f => f.CreationDate, dateGt);

            var cars = _cars.Find(filterCount).ToList();
            var calc = cars.Sum(f => f.SoldValue);

            return calc;
        }
    }
}
