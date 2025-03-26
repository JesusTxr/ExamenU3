using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Producto
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public int CantidadStock { get; set; }
    public double PrecioUnitario { get; set; }
}

class Inventario
{
    private string _rutaArchivo;
    private List<Producto> _productos;

    public Inventario(string rutaArchivo)
    {
        _rutaArchivo = rutaArchivo;
        CargarInventario();
    }

    private void CargarInventario()
    {
        if (File.Exists(_rutaArchivo))
        {
            string contenido = File.ReadAllText(_rutaArchivo);
            _productos = JsonConvert.DeserializeObject<List<Producto>>(contenido) ?? new List<Producto>();
        }
        else
        {
            _productos = new List<Producto>();
        }
    }

    public void AgregarProducto(Producto producto)
    {
        _productos.Add(producto);
        GuardarInventario();
    }

    private void GuardarInventario()
    {
        string json = JsonConvert.SerializeObject(_productos, Formatting.Indented);
        File.WriteAllText(_rutaArchivo, json);
    }

    public void MostrarInventario()
    {
        Console.WriteLine("\nInventario de productos:");
        foreach (var producto in _productos)
        {
            Console.WriteLine($"Código: {producto.Codigo}, Nombre: {producto.Nombre}, Stock: {producto.CantidadStock}, Precio: {producto.PrecioUnitario:C}");
        }
    }

    public Producto BuscarProducto(string criterio)
    {
        return _productos.Find(p => p.Codigo.Equals(criterio, StringComparison.OrdinalIgnoreCase) || 
                                    p.Nombre.Equals(criterio, StringComparison.OrdinalIgnoreCase));
    }

    public bool ComprarProducto(string criterio, int cantidad)
    {
        Producto producto = BuscarProducto(criterio);
        if (producto != null && producto.CantidadStock >= cantidad)
        {
            producto.CantidadStock -= cantidad;
            GuardarInventario();
            Console.WriteLine($"Compra realizada: {cantidad} unidades de {producto.Nombre}.");
            return true;
        }
        Console.WriteLine("Producto no encontrado o stock insuficiente.");
        return false;
    }

    public void PagarProducto(double total)
    {
        double montoIngresado = 0;
        while (montoIngresado < total)
        {
            Console.Write("Ingrese dinero (MXN): ");
            double ingreso = double.Parse(Console.ReadLine());
            montoIngresado += ingreso;
            Console.WriteLine($"Total ingresado: {montoIngresado:C}");
        }
        Console.WriteLine($"Pago completado. Cambio: {(montoIngresado - total):C}");
    }
}

class Program
{
    static void Main()
    {
        string rutaArchivo = "inventario.json";
        Inventario inventario = new Inventario(rutaArchivo);

        while (true)
        {
            Console.WriteLine("\nOpciones:");
            Console.WriteLine("1. Agregar producto");
            Console.WriteLine("2. Mostrar inventario");
            Console.WriteLine("3. Buscar producto");
            Console.WriteLine("4. Comprar producto");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            int opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1:
                    Console.Write("Nombre: ");
                    string nombre = Console.ReadLine();
                    Console.Write("Código: ");
                    string codigo = Console.ReadLine();
                    Console.Write("Cantidad en stock: ");
                    int stock = int.Parse(Console.ReadLine());
                    Console.Write("Precio unitario: ");
                    double precio = double.Parse(Console.ReadLine());
                    inventario.AgregarProducto(new Producto { Codigo = codigo, Nombre = nombre, CantidadStock = stock, PrecioUnitario = precio });
                    break;

                case 2:
                    inventario.MostrarInventario();
                    break;

                case 3:
                    Console.Write("Ingrese código o nombre del producto: ");
                    string criterio = Console.ReadLine();
                    Producto productoEncontrado = inventario.BuscarProducto(criterio);
                    if (productoEncontrado != null)
                    {
                        Console.WriteLine($"Producto encontrado: {productoEncontrado.Nombre}, Stock: {productoEncontrado.CantidadStock}, Precio: {productoEncontrado.PrecioUnitario:C}");
                    }
                    else
                    {
                        Console.WriteLine("Producto no encontrado.");
                    }
                    break;

                case 4:
                    break;
                case 5:
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente nuevamente.");
                    break;
            }
        }
    }
}