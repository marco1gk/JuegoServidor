using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServicioJuego;

namespace Host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(ImplementacionServicio)))
            {
                host.Open();
                Console.WriteLine("El servicio esta corriendo");
                Console.WriteLine("Servicio iniciado en net.tcp://localhost:8000/ServiceImplementation");
                Console.ReadLine(); 
            }

        }
    }
}
