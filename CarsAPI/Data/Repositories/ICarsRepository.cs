using CarsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarsAPI.Data.Repositories
{
    public interface ICarsRepository
    {
        object Filter(
            string model, 
            string marca,
            int year, 
            bool? sold, 
            double valueGt, 
            double valueLt, 
            DateTime dateGt,
            DateTime dateLt,
            int? page);

        object FilterPeriodo(
            bool sold,
            DateTime dateGt,
            DateTime dateLt);

        object FilterSum(
            DateTime dateGt,
            DateTime dateLt);

        void AddCar(Car car);

        void UpdateCar(String id, Car updateCar);

        IEnumerable<Car> GetCar();

        Car GetCar(string id);

        void DeleteCar(string id);
    }
}
