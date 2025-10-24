using Million.API.Domain;
using Million.API.Repository;

namespace Million.API.Services
{
    /// <summary>
    /// Service for seeding initial data into the database
    /// </summary>
    public class DataSeederService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyImageRepository _imageRepository;
        private readonly IPropertyTraceRepository _traceRepository;
        private readonly ILogger<DataSeederService> _logger;

        public DataSeederService(
            IOwnerRepository ownerRepository,
            IPropertyRepository propertyRepository,
            IPropertyImageRepository imageRepository,
            IPropertyTraceRepository traceRepository,
            ILogger<DataSeederService> logger)
        {
            _ownerRepository = ownerRepository;
            _propertyRepository = propertyRepository;
            _imageRepository = imageRepository;
            _traceRepository = traceRepository;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database with initial mock data
        /// </summary>
        public async Task SeedDataAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                // Check if data already exists
                var existingOwners = await _ownerRepository.GetAllAsync();
                if (existingOwners.Any())
                {
                    _logger.LogInformation("Database already contains data. Skipping seed.");
                    return;
                }

                // Seed Owners
                var owners = await SeedOwnersAsync();
                _logger.LogInformation($"Seeded {owners.Count} owners");

                // Seed Properties
                var properties = await SeedPropertiesAsync(owners);
                _logger.LogInformation($"Seeded {properties.Count} properties");

                // Seed Property Images
                var images = await SeedPropertyImagesAsync(properties);
                _logger.LogInformation($"Seeded {images.Count} property images");

                // Seed Property Traces
                var traces = await SeedPropertyTracesAsync(properties);
                _logger.LogInformation($"Seeded {traces.Count} property traces");

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding database");
                throw;
            }
        }

        /// <summary>
        /// Deletes all data and reseeds the database
        /// </summary>
        public async Task ResetAndSeedAsync()
        {
            try
            {
                _logger.LogWarning("Starting database reset - ALL DATA WILL BE DELETED");

                // Delete all data
                await DeleteAllDataAsync();
                _logger.LogInformation("All data deleted successfully");

                // Seed new data
                var owners = await SeedOwnersAsync();
                _logger.LogInformation($"Seeded {owners.Count} owners");

                var properties = await SeedPropertiesAsync(owners);
                _logger.LogInformation($"Seeded {properties.Count} properties");

                var images = await SeedPropertyImagesAsync(properties);
                _logger.LogInformation($"Seeded {images.Count} property images");

                var traces = await SeedPropertyTracesAsync(properties);
                _logger.LogInformation($"Seeded {traces.Count} property traces");

                _logger.LogInformation("Database reset and seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting database");
                throw;
            }
        }

        /// <summary>
        /// Deletes all data from all collections
        /// </summary>
        private async Task DeleteAllDataAsync()
        {
            var traces = await _traceRepository.GetAllAsync();
            foreach (var trace in traces)
            {
                await _traceRepository.DeleteAsync(trace.IdPropertyTrace);
            }

            var images = await _imageRepository.GetAllAsync();
            foreach (var image in images)
            {
                await _imageRepository.DeleteAsync(image.IdPropertyImage);
            }

            var properties = await _propertyRepository.GetAllAsync();
            foreach (var property in properties)
            {
                await _propertyRepository.DeleteAsync(property.IdProperty);
            }

            var owners = await _ownerRepository.GetAllAsync();
            foreach (var owner in owners)
            {
                await _ownerRepository.DeleteAsync(owner.IdOwner);
            }
        }

        private async Task<List<Owner>> SeedOwnersAsync()
        {
            var owners = new List<Owner>
            {
                new Owner
                {
                    Name = "Juan Pérez García",
                    Address = "Calle Principal #123, Bogotá",
                    Photo = "https://i.pravatar.cc/150?img=12",
                    Birthday = new DateTime(1985, 3, 15)
                },
                new Owner
                {
                    Name = "María Rodríguez López",
                    Address = "Carrera 7 #45-67, Medellín",
                    Photo = "https://i.pravatar.cc/150?img=5",
                    Birthday = new DateTime(1990, 7, 22)
                },
                new Owner
                {
                    Name = "Carlos Martínez Sánchez",
                    Address = "Avenida El Dorado #89-12, Cali",
                    Photo = "https://i.pravatar.cc/150?img=33",
                    Birthday = new DateTime(1978, 11, 8)
                },
                new Owner
                {
                    Name = "Ana Gómez Fernández",
                    Address = "Calle 100 #15-20, Cartagena",
                    Photo = "https://i.pravatar.cc/150?img=9",
                    Birthday = new DateTime(1995, 2, 14)
                },
                new Owner
                {
                    Name = "Luis Fernando Castro",
                    Address = "Transversal 45 #67-89, Barranquilla",
                    Photo = "https://i.pravatar.cc/150?img=52",
                    Birthday = new DateTime(1982, 9, 30)
                },
                new Owner
                {
                    Name = "Diana Patricia Morales",
                    Address = "Diagonal 25 #34-56, Bucaramanga",
                    Photo = "https://i.pravatar.cc/150?img=20",
                    Birthday = new DateTime(1988, 5, 18)
                },
                new Owner
                {
                    Name = "Roberto Silva Ramírez",
                    Address = "Calle 50 #23-45, Pereira",
                    Photo = "https://i.pravatar.cc/150?img=60",
                    Birthday = new DateTime(1975, 12, 3)
                },
                new Owner
                {
                    Name = "Laura Sofía Jiménez",
                    Address = "Carrera 15 #78-90, Manizales",
                    Photo = "https://i.pravatar.cc/150?img=28",
                    Birthday = new DateTime(1992, 4, 25)
                }
            };

            foreach (var owner in owners)
            {
                await _ownerRepository.CreateAsync(owner);
            }

            return owners;
        }

        private async Task<List<Property>> SeedPropertiesAsync(List<Owner> owners)
        {
            var properties = new List<Property>
            {
                new Property
                {
                    Name = "Apartamento Moderno en Chapinero",
                    Address = "Calle 63 #7-45, Chapinero, Bogotá",
                    Price = 450000000,
                    CodeInternal = "PROP-2024-001",
                    Year = 2022,
                    IdOwner = owners[0].IdOwner
                },
                new Property
                {
                    Name = "Casa Campestre en La Calera",
                    Address = "Vereda El Salitre, Km 5, La Calera",
                    Price = 850000000,
                    CodeInternal = "PROP-2024-002",
                    Year = 2020,
                    IdOwner = owners[0].IdOwner
                },
                new Property
                {
                    Name = "Penthouse El Poblado Premium",
                    Address = "Carrera 43A #10-50, El Poblado, Medellín",
                    Price = 1200000000,
                    CodeInternal = "PROP-2024-003",
                    Year = 2023,
                    IdOwner = owners[1].IdOwner
                },
                new Property
                {
                    Name = "Apartamento Vista al Mar",
                    Address = "Avenida San Martín #8-120, Bocagrande, Cartagena",
                    Price = 680000000,
                    CodeInternal = "PROP-2024-004",
                    Year = 2021,
                    IdOwner = owners[3].IdOwner
                },
                new Property
                {
                    Name = "Oficina Centro Internacional",
                    Address = "Carrera 7 #26-20, Piso 15, Bogotá",
                    Price = 320000000,
                    CodeInternal = "PROP-2024-005",
                    Year = 2019,
                    IdOwner = owners[2].IdOwner
                },
                new Property
                {
                    Name = "Local Comercial Unicentro",
                    Address = "Centro Comercial Unicentro, Local 234, Bogotá",
                    Price = 280000000,
                    CodeInternal = "PROP-2024-006",
                    Year = 2018,
                    IdOwner = owners[2].IdOwner
                },
                new Property
                {
                    Name = "Casa Conjunto Cerrado Norte",
                    Address = "Calle 170 #54-23, Conjunto Reserva del Bosque",
                    Price = 620000000,
                    CodeInternal = "PROP-2024-007",
                    Year = 2021,
                    IdOwner = owners[4].IdOwner
                },
                new Property
                {
                    Name = "Apartamento Laureles Medellín",
                    Address = "Circular 1 #70-45, Laureles, Medellín",
                    Price = 380000000,
                    CodeInternal = "PROP-2024-008",
                    Year = 2020,
                    IdOwner = owners[1].IdOwner
                },
                new Property
                {
                    Name = "Finca Cafetera Eje Cafetero",
                    Address = "Vereda La Esperanza, Montenegro, Quindío",
                    Price = 950000000,
                    CodeInternal = "PROP-2024-009",
                    Year = 2015,
                    IdOwner = owners[5].IdOwner
                },
                new Property
                {
                    Name = "Studio Amoblado Rosales",
                    Address = "Carrera 7 #72-34, Rosales, Bogotá",
                    Price = 290000000,
                    CodeInternal = "PROP-2024-010",
                    Year = 2022,
                    IdOwner = owners[6].IdOwner
                },
                new Property
                {
                    Name = "Loft Industrial Provenza",
                    Address = "Calle 8 #43E-65, Provenza, Medellín",
                    Price = 520000000,
                    CodeInternal = "PROP-2024-011",
                    Year = 2023,
                    IdOwner = owners[7].IdOwner
                },
                new Property
                {
                    Name = "Casa Colonial Centro Histórico",
                    Address = "Calle del Arsenal #45, Centro, Cartagena",
                    Price = 1500000000,
                    CodeInternal = "PROP-2024-012",
                    Year = 1890,
                    IdOwner = owners[3].IdOwner
                },
                new Property
                {
                    Name = "Apartamento Parque 93",
                    Address = "Calle 93B #11-45, Chicó, Bogotá",
                    Price = 580000000,
                    CodeInternal = "PROP-2024-013",
                    Year = 2021,
                    IdOwner = owners[4].IdOwner
                },
                new Property
                {
                    Name = "Casa Moderna Envigado",
                    Address = "Calle 38 Sur #28-45, Envigado, Antioquia",
                    Price = 720000000,
                    CodeInternal = "PROP-2024-014",
                    Year = 2022,
                    IdOwner = owners[5].IdOwner
                },
                new Property
                {
                    Name = "Apartamento Bucaramanga Norte",
                    Address = "Carrera 27 #151-34, Cabecera, Bucaramanga",
                    Price = 410000000,
                    CodeInternal = "PROP-2024-015",
                    Year = 2020,
                    IdOwner = owners[6].IdOwner
                }
            };

            foreach (var property in properties)
            {
                await _propertyRepository.CreateAsync(property);
            }

            return properties;
        }

        private async Task<List<PropertyImage>> SeedPropertyImagesAsync(List<Property> properties)
        {
            var imageUrls = new List<string>
            {
                "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800",
                "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800",
                "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800",
                "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800",
                "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800",
                "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800",
                "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800",
                "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?w=800",
                "https://images.unsplash.com/photo-1600607687644-c7171b42498f?w=800",
                "https://images.unsplash.com/photo-1600607688969-a5bfcd646154?w=800"
            };

            var images = new List<PropertyImage>();
            var random = new Random(42); // Seed for reproducibility

            foreach (var property in properties)
            {
                // Add 2-4 images per property
                var imageCount = random.Next(2, 5);
                for (int i = 0; i < imageCount; i++)
                {
                    var image = new PropertyImage
                    {
                        IdProperty = property.IdProperty,
                        File = imageUrls[random.Next(imageUrls.Count)],
                        Enabled = i == 0 // First image enabled by default
                    };
                    images.Add(image);
                }
            }

            foreach (var image in images)
            {
                await _imageRepository.CreateAsync(image);
            }

            return images;
        }

        private async Task<List<PropertyTrace>> SeedPropertyTracesAsync(List<Property> properties)
        {
            var traces = new List<PropertyTrace>();
            var random = new Random(42);

            var traceNames = new[]
            {
                "Venta inicial",
                "Actualización de precio",
                "Renovación",
                "Remodelación",
                "Cambio de propietario",
                "Avalúo comercial",
                "Actualización catastral"
            };

            foreach (var property in properties)
            {
                // Add 1-3 traces per property
                var traceCount = random.Next(1, 4);
                var baseDate = DateTime.Now.AddYears(-2);

                for (int i = 0; i < traceCount; i++)
                {
                    var trace = new PropertyTrace
                    {
                        IdProperty = property.IdProperty,
                        DateSale = baseDate.AddMonths(random.Next(1, 24)),
                        Name = traceNames[random.Next(traceNames.Length)],
                        Value = property.Price * (decimal)(0.9 + random.NextDouble() * 0.2), // ±10% variation
                        Tax = property.Price * 0.01m * random.Next(1, 5) // 1-5% tax
                    };
                    traces.Add(trace);
                }
            }

            foreach (var trace in traces)
            {
                await _traceRepository.CreateAsync(trace);
            }

            return traces;
        }
    }
}
