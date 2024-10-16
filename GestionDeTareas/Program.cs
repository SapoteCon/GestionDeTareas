using System;
using System.Collections.Generic;
using System.Linq;

using System;
using System.Collections.Generic;

public class Tarea
{
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int Prioridad { get; set; } // 1 = Alta, 2 = Media, 3 = Baja
    public DateTime FechaVencimiento { get; set; }

    public Tarea(string titulo, string descripcion, int prioridad, DateTime fechaVencimiento)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad;
        FechaVencimiento = fechaVencimiento;
    }

    public override string ToString()
    {
        return $"{Titulo} - {Descripcion} | Prioridad: {Prioridad} | Vence: {FechaVencimiento.ToShortDateString()}";
    }
}

public class Categoria
{
    public string Nombre { get; set; }
    public List<Tarea> Tareas { get; set; }
    public List<Categoria> Subcategorias { get; set; }

    public Categoria(string nombre)
    {
        Nombre = nombre;
        Tareas = new List<Tarea>();
        Subcategorias = new List<Categoria>();
    }

    public void Mostrar(int nivel = 0)
    {
        string indentacion = new string(' ', nivel * 2);
        Console.WriteLine($"{indentacion}- {Nombre}");

        foreach (var tarea in Tareas)
        {
            Console.WriteLine($"{indentacion}  * {tarea}");
        }

        foreach (var subcategoria in Subcategorias)
        {
            subcategoria.Mostrar(nivel + 1);
        }
    }
}

class Program
{
    static List<Tarea> ListaTareas = new List<Tarea>();
    static Stack<Tarea> PilaDeshacer = new Stack<Tarea>();
    static Stack<Tarea> PilaRehacer = new Stack<Tarea>();
    static Queue<Tarea> ColaUrgente = new Queue<Tarea>();
    static Categoria ArbolCategorias = new Categoria("Raíz");

    static void Main(string[] args)
    {
        bool ejecutando = true;
        while (ejecutando)
        {
            MostrarMenu();
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    AgregarTarea();
                    break;
                case "2":
                    EliminarTarea();
                    break;
                case "3":
                    ModificarTarea();
                    break;
                case "4":
                    ListarTareas();
                    break;
                case "5":
                    AgregarTareaUrgente();
                    break;
                case "6":
                    ProcesarTareaUrgente();
                    break;
                case "7":
                    DeshacerAccion();
                    break;
                case "8":
                    RehacerAccion();
                    break;
                case "9":
                    OrganizarEnCategorias();
                    break;
                case "10":
                    AgregarTareaACategoria();
                    break;
                case "11":
                    ListarCategorias(ArbolCategorias);
                    break;
                case "0":
                    ejecutando = false;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        }
    }

    static void MostrarMenu()
    {
        Console.WriteLine("\n--- Menú de Gestión de Tareas ---");
        Console.WriteLine("1. Agregar Tarea");
        Console.WriteLine("2. Eliminar Tarea");
        Console.WriteLine("3. Modificar Tarea");
        Console.WriteLine("4. Listar Tareas");
        Console.WriteLine("5. Agregar Tarea Urgente");
        Console.WriteLine("6. Procesar Tarea Urgente");
        Console.WriteLine("7. Deshacer Acción");
        Console.WriteLine("8. Rehacer Acción");
        Console.WriteLine("9. Organizar en Categorías/Subcategorías");
        Console.WriteLine("10. Agregar Tarea a Categoría/Subcategoría");
        Console.WriteLine("11. Listar Categorías");
        Console.WriteLine("0. Salir");
        Console.Write("Seleccione una opción: ");
    }

    static void AgregarTarea()
    {
        try
        {
            Console.Write("Título de la tarea: ");
            string titulo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("El título no puede estar vacío.");

            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine();

            Console.Write("Prioridad (1 = Alta, 2 = Media, 3 = Baja): ");
            if (!int.TryParse(Console.ReadLine(), out int prioridad) || prioridad < 1 || prioridad > 3)
                throw new ArgumentException("Prioridad inválida. Debe ser 1, 2 o 3.");

            Console.Write("Fecha de vencimiento (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime fechaVencimiento))
                throw new ArgumentException("Fecha de vencimiento no válida.");

            Tarea nuevaTarea = new Tarea(titulo, descripcion, prioridad, fechaVencimiento);
            ListaTareas.Add(nuevaTarea);
            GuardarHistorial(nuevaTarea);
            Console.WriteLine("Tarea agregada correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al agregar la tarea: {ex.Message}");
        }
    }

    static void EliminarTarea()
    {
        try
        {
            ListarTareas();
            Console.Write("Índice de la tarea a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 0 || indice >= ListaTareas.Count)
                throw new ArgumentException("Índice inválido.");

            Tarea tareaEliminada = ListaTareas[indice];
            ListaTareas.RemoveAt(indice);
            GuardarHistorial(tareaEliminada);
            Console.WriteLine("Tarea eliminada.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar la tarea: {ex.Message}");
        }
    }

    static void ModificarTarea()
    {
        try
        {
            ListarTareas();
            Console.Write("Índice de la tarea a modificar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 0 || indice >= ListaTareas.Count)
                throw new ArgumentException("Índice inválido.");

            Console.Write("Nuevo título: ");
            string nuevoTitulo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nuevoTitulo)) throw new ArgumentException("El título no puede estar vacío.");

            Console.Write("Nueva descripción: ");
            string nuevaDescripcion = Console.ReadLine();

            Console.Write("Nueva prioridad (1 = Alta, 2 = Media, 3 = Baja): ");
            if (!int.TryParse(Console.ReadLine(), out int nuevaPrioridad) || nuevaPrioridad < 1 || nuevaPrioridad > 3)
                throw new ArgumentException("Prioridad inválida. Debe ser 1, 2 o 3.");

            Console.Write("Nueva fecha de vencimiento (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime nuevaFecha))
                throw new ArgumentException("Fecha de vencimiento no válida.");

            Tarea tarea = ListaTareas[indice];
            tarea.Titulo = nuevoTitulo;
            tarea.Descripcion = nuevaDescripcion;
            tarea.Prioridad = nuevaPrioridad;
            tarea.FechaVencimiento = nuevaFecha;
            GuardarHistorial(tarea);
            Console.WriteLine("Tarea modificada.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al modificar la tarea: {ex.Message}");
        }
    }

    static void ListarTareas()
    {
        if (ListaTareas.Count == 0)
        {
            Console.WriteLine("No hay tareas.");
            return;
        }

        for (int i = 0; i < ListaTareas.Count; i++)
        {
            Console.WriteLine($"{i}. {ListaTareas[i]}");
        }
    }

    static void AgregarTareaUrgente()
    {
        try
        {
            Console.Write("Título de la tarea urgente: ");
            string titulo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("El título no puede estar vacío.");

            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine();

            Console.Write("Prioridad (1 = Alta, 2 = Media, 3 = Baja): ");
            if (!int.TryParse(Console.ReadLine(), out int prioridad) || prioridad < 1 || prioridad > 3)
                throw new ArgumentException("Prioridad inválida. Debe ser 1, 2 o 3.");

            Console.Write("Fecha de vencimiento (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime fechaVencimiento))
                throw new ArgumentException("Fecha de vencimiento no válida.");

            Tarea tareaUrgente = new Tarea(titulo, descripcion, prioridad, fechaVencimiento);
            ColaUrgente.Enqueue(tareaUrgente);
            Console.WriteLine("Tarea urgente agregada a la cola.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al agregar la tarea urgente: {ex.Message}");
        }
    }

    static void ProcesarTareaUrgente()
    {
        if (ColaUrgente.Count > 0)
        {
            Tarea tareaProcesada = ColaUrgente.Dequeue();
            Console.WriteLine($"Procesando tarea urgente: {tareaProcesada}");
        }
        else
        {
            Console.WriteLine("No hay tareas urgentes en la cola.");
        }
    }

    static void DeshacerAccion()
    {
        if (PilaDeshacer.Count > 0)
        {
            Tarea tareaDeshecha = PilaDeshacer.Pop();
            ListaTareas.Remove(tareaDeshecha);
            PilaRehacer.Push(tareaDeshecha);
            Console.WriteLine("Acción deshecha.");
        }
        else
        {
            Console.WriteLine("No hay acciones para deshacer.");
        }
    }

    static void RehacerAccion()
    {
        if (PilaRehacer.Count > 0)
        {
            Tarea tareaRehecha = PilaRehacer.Pop();
            ListaTareas.Add(tareaRehecha);
            PilaDeshacer.Push(tareaRehecha);
            Console.WriteLine("Acción rehecha.");
        }
        else
        {
            Console.WriteLine("No hay acciones para rehacer.");
        }
    }

    static void GuardarHistorial(Tarea tarea)
    {
        PilaDeshacer.Push(tarea);
    }

    static void OrganizarEnCategorias()
    {
        try
        {
            Console.Write("Nombre de la nueva categoría o subcategoría: ");
            string nombreCategoria = Console.ReadLine();

            Console.WriteLine("Seleccione la categoría padre:");
            ListarCategorias(ArbolCategorias);
            Console.Write("Índice de la categoría padre (0 para la raíz): ");
            int indiceCategoriaPadre;

            if (!int.TryParse(Console.ReadLine(), out indiceCategoriaPadre))
            {
                Console.WriteLine("Entrada no válida. Debe ser un número.");
                return;
            }

            Categoria categoriaPadre = EncontrarCategoriaPorIndice(ArbolCategorias, ref indiceCategoriaPadre);

            if (categoriaPadre != null)
            {
                Categoria nuevaCategoria = new Categoria(nombreCategoria);
                categoriaPadre.Subcategorias.Add(nuevaCategoria);
                Console.WriteLine($"Categoría '{nombreCategoria}' agregada correctamente.");
            }
            else
            {
                Console.WriteLine("Índice de categoría no válido.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al organizar en categorías: {ex.Message}");
        }
    }


    static void AgregarTareaACategoria()
    {
        try
        {
            if (ListaTareas.Count == 0)
            {
                Console.WriteLine("No hay tareas disponibles para agregar.");
                return;
            }

            Console.WriteLine("Seleccione la categoría a la que desea agregar la tarea:");
            ListarCategorias(ArbolCategorias);
            Console.Write("Índice de la categoría: ");
            int indiceCategoria;

            if (!int.TryParse(Console.ReadLine(), out indiceCategoria))
            {
                Console.WriteLine("Entrada no válida. Debe ser un número.");
                return;
            }

            Categoria categoriaSeleccionada = EncontrarCategoriaPorIndice(ArbolCategorias, ref indiceCategoria);

            if (categoriaSeleccionada != null)
            {
                ListarTareas();
                Console.Write("Índice de la tarea a agregar: ");
                int indiceTarea;

                if (!int.TryParse(Console.ReadLine(), out indiceTarea))
                {
                    Console.WriteLine("Entrada no válida. Debe ser un número.");
                    return;
                }

                if (indiceTarea >= 0 && indiceTarea < ListaTareas.Count)
                {
                    Tarea tareaSeleccionada = ListaTareas[indiceTarea];
                    categoriaSeleccionada.Tareas.Add(tareaSeleccionada);
                    Console.WriteLine($"Tarea '{tareaSeleccionada.Titulo}' agregada a la categoría '{categoriaSeleccionada.Nombre}'.");
                }
                else
                {
                    Console.WriteLine("Índice de tarea no válido.");
                }
            }
            else
            {
                Console.WriteLine("Índice de categoría no válido.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al agregar tarea a la categoría: {ex.Message}");
        }
    }

    static Categoria EncontrarCategoriaPorIndice(Categoria categoria, ref int indice)
    {
        if (indice == 0) return categoria; 

        foreach (var subcategoria in categoria.Subcategorias)
        {
            indice--; 
            var resultado = EncontrarCategoriaPorIndice(subcategoria, ref indice);
            if (resultado != null)
            {
                return resultado; 
            }
        }

        return null; 
    }

    static void ListarCategorias(Categoria categoria, int nivel = 0)
    {
        string indentacion = new string(' ', nivel * 2);
        Console.WriteLine($"{indentacion}- {categoria.Nombre}");

        for (int i = 0; i < categoria.Subcategorias.Count; i++)
        {
            ListarCategorias(categoria.Subcategorias[i], nivel + 1);
        }
    }

}
