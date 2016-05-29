using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Text.RegularExpressions;

namespace TPV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Numero de productos añadidos a la ventana de venta
        int numProductosResumenVenta;
        // id de cabecera de venta
        int idCabeceraVenta;
        // Datos de la tabla resumen de venta
        DataTable dataTableResumenVenta;

        // Numero de productos añadidos a la ventana de compra
        int numProductosResumenCompra;
        // id de cabecera de compra
        int idCabeceraCompra;
        // Datos de la tabla resumen de compra
        DataTable dataTableResumenCompra;

        private TPV.TPVDataSetTableAdapters.ProductosTableAdapter tPVDataSetProductosTableAdapter;
        private TPV.TPVDataSetTableAdapters.ClientesCompradoresTableAdapter tPVDataSetClientesCompradoresTableAdapter;
        private TPV.TPVDataSetTableAdapters.ClientesVendedoresTableAdapter tPVDataSetClientesVendedoresTableAdapter;
        private TPV.TPVDataSetTableAdapters.LineasVentasTableAdapter tPVDataSetLineasVentasTableAdapter;
        private TPV.TPVDataSetTableAdapters.CabecerasVentasTableAdapter tPVDataSetCabecerasVentasTableAdapter;
        private TPV.TPVDataSetTableAdapters.LineasComprasTableAdapter tPVDataSetLineasComprasTableAdapter;
        private TPV.TPVDataSetTableAdapters.CabecerasComprasTableAdapter tPVDataSetCabecerasComprasTableAdapter;

        private TPV.TPVDataSet tPVDataSet;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            numProductosResumenVenta = 0;
            numProductosResumenCompra = 0;

            tPVDataSet = ((TPV.TPVDataSet)(this.FindResource("tPVDataSet")));

            // Load data into the table Productos. You can modify this code as needed.
            tPVDataSetProductosTableAdapter = new TPV.TPVDataSetTableAdapters.ProductosTableAdapter();
            tPVDataSetProductosTableAdapter.Fill(tPVDataSet.Productos);
            System.Windows.Data.CollectionViewSource productosViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productosViewSource")));
            productosViewSource.View.MoveCurrentToFirst();
            // Load data into the table ClientesCompradores. You can modify this code as needed.
            tPVDataSetClientesCompradoresTableAdapter = new TPV.TPVDataSetTableAdapters.ClientesCompradoresTableAdapter();
            tPVDataSetClientesCompradoresTableAdapter.Fill(tPVDataSet.ClientesCompradores);
            System.Windows.Data.CollectionViewSource clientesCompradoresViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientesCompradoresViewSource")));
            clientesCompradoresViewSource.View.MoveCurrentToFirst();
            // Load data into the table ClientesVendedores. You can modify this code as needed.
            tPVDataSetClientesVendedoresTableAdapter = new TPV.TPVDataSetTableAdapters.ClientesVendedoresTableAdapter();
            tPVDataSetClientesVendedoresTableAdapter.Fill(tPVDataSet.ClientesVendedores);
            System.Windows.Data.CollectionViewSource clientesVendedoresViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientesVendedoresViewSource")));
            clientesVendedoresViewSource.View.MoveCurrentToFirst();
            // Load data into the table CabecerasVentas. You can modify this code as needed.
            tPVDataSetCabecerasVentasTableAdapter = new TPV.TPVDataSetTableAdapters.CabecerasVentasTableAdapter();
            tPVDataSetCabecerasVentasTableAdapter.Fill(tPVDataSet.CabecerasVentas);
            System.Windows.Data.CollectionViewSource cabecerasVentasViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("cabecerasVentasViewSource")));
            cabecerasVentasViewSource.View.MoveCurrentToFirst();
            // Load data into the table LineasVentas. You can modify this code as needed.
            tPVDataSetLineasVentasTableAdapter = new TPV.TPVDataSetTableAdapters.LineasVentasTableAdapter();
            tPVDataSetLineasVentasTableAdapter.Fill(tPVDataSet.LineasVentas);
            System.Windows.Data.CollectionViewSource lineasVentasViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("lineasVentasViewSource")));
            lineasVentasViewSource.View.MoveCurrentToFirst();
            // Load data into the table CabecerasCompras. You can modify this code as needed.
            tPVDataSetCabecerasComprasTableAdapter = new TPV.TPVDataSetTableAdapters.CabecerasComprasTableAdapter();
            tPVDataSetCabecerasComprasTableAdapter.Fill(tPVDataSet.CabecerasCompras);
            System.Windows.Data.CollectionViewSource cabecerasComprasViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("cabecerasComprasViewSource")));
            cabecerasComprasViewSource.View.MoveCurrentToFirst();
            // Load data into the table LineasCompras. You can modify this code as needed.
            tPVDataSetLineasComprasTableAdapter = new TPV.TPVDataSetTableAdapters.LineasComprasTableAdapter();
            tPVDataSetLineasComprasTableAdapter.Fill(tPVDataSet.LineasCompras);
            System.Windows.Data.CollectionViewSource lineasComprasViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("lineasComprasViewSource")));
            lineasComprasViewSource.View.MoveCurrentToFirst();

            //Recargo las categorias
            cargarCategorias(cbxCategoriasStock);
            cargarCategorias(cbxAñadirCategoriaStock);

            // Crear la tabla de resumen de venta y asignarla al datagrid
            dataTableResumenVenta = crearDataTableResumen("ResumenVenta");
            dataTableResumenVenta.RowDeleted += new DataRowChangeEventHandler(resumenVentaRow_Deleted);

            resumenVenta.DataContext = dataTableResumenVenta.DefaultView;

            // Crear la tabla de resumen de venta y asignarla al datagrid
            dataTableResumenCompra = crearDataTableResumen("ResumenCompra");
            dataTableResumenCompra.RowDeleted += new DataRowChangeEventHandler(resumenCompraRow_Deleted);

            resumenCompra.DataContext = dataTableResumenCompra.DefaultView;

            //Esconde el tabControl al iniciar
            tabControl.Visibility = Visibility.Hidden;

            //Deseleccionar los combobox de cliente de compra y venta
            cbxClienteCompras.SelectedItem = cbxClienteVentas.SelectedItem = null;


        }

        #region Stock

        private void btnStock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabStock.IsSelected = true;
        }

        private void cbxCategoriasStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategoria = cbxCategoriasStock.SelectedItem as DataRowView;

            if (selectedCategoria != null)
                tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "Categoria like \'%" + selectedCategoria["Categoria"] + "%\'";
            else
                tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "";
        }

        private void btnAñadirStock_Click(object sender, RoutedEventArgs e)
        {
            cbxCategoriasStock.SelectedItem = null;
            lvStock.SelectedItem = null;

            btnAñadirConfirmarStock.Visibility = Visibility.Visible;
            btnAñadirCancelarStock.Visibility = Visibility.Visible;

            tbxNombreStock.Clear();
            tbxDescripcionStock.Clear();
            tbxPrecioStock.Clear();
            tbxCantidadStock.Clear();

            btnAñadirStock.IsEnabled = false;
            btnEliminarStock.IsEnabled = false;
            btnModificarStock.IsEnabled = false;

            cbxAñadirCategoriaStock.IsHitTestVisible = true;

            tbxCantidadStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxNombreStock.IsReadOnly = tbxPrecioStock.IsReadOnly = false;
        }

        private void btnAñadirConfirmarStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = tPVDataSet.Tables["Productos"].NewRow();
                row["Nombre"] = tbxNombreStock.Text;
                row["Descripcion"] = tbxDescripcionStock.Text;
                row["Precio"] = Convert.ToDecimal(tbxPrecioStock.Text.Replace(".", ","));
                row["Cantidad"] = Convert.ToInt32(tbxCantidadStock.Text);
                if (cbxAñadirCategoriaStock.SelectedValue != null)
                    row["Categoria"] = cbxAñadirCategoriaStock.SelectedValue.ToString();
                else if (cbxAñadirCategoriaStock.Text != "")
                    row["Categoria"] = cbxAñadirCategoriaStock.Text;
                else
                    row["Categoria"] = null;

                tPVDataSet.Tables["Productos"].Rows.Add(row);

                tPVDataSetProductosTableAdapter.Update(tPVDataSet);

                tbxNombreStock.Clear();
                tbxDescripcionStock.Clear();
                tbxPrecioStock.Clear();
                tbxCantidadStock.Clear();
                cbxAñadirCategoriaStock.Text = "";

                btnAñadirStock.IsEnabled = true;
                btnEliminarStock.IsEnabled = true;
                btnModificarStock.IsEnabled = true;

                btnAñadirConfirmarStock.Visibility = Visibility.Hidden;
                btnAñadirCancelarStock.Visibility = Visibility.Hidden;

                cbxAñadirCategoriaStock.IsHitTestVisible = false;

                tbxCantidadStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxNombreStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;

                cargarCategorias(cbxCategoriasStock);

                cargarCategorias(cbxAñadirCategoriaStock);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAñadirCancelarStock_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirStock.IsEnabled = true;
            btnEliminarStock.IsEnabled = true;
            btnModificarStock.IsEnabled = true;

            btnAñadirConfirmarStock.Visibility = Visibility.Hidden;
            btnAñadirCancelarStock.Visibility = Visibility.Hidden;

            cbxCategoriasStock.Text = "";

            cbxAñadirCategoriaStock.IsHitTestVisible = false;

            tbxCantidadStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxNombreStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;
        }

        private void btnEliminarStock_Click(object sender, RoutedEventArgs e)
        {
            btnEliminarConfirmarStock.Visibility = Visibility.Visible;
            btnEliminarCancelarStock.Visibility = Visibility.Visible;

            btnAñadirStock.IsEnabled = false;
            btnEliminarStock.IsEnabled = false;
            btnModificarStock.IsEnabled = false;
        }

        private void btnEliminarConfirmarStock_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvStock.SelectedItem as DataRowView;

            try
            {
                DataRow row = tPVDataSet.Tables["Productos"].Rows.Find(selecteditem["id"]);

                row.Delete();

                tPVDataSetProductosTableAdapter.Update(tPVDataSet);

                btnAñadirStock.IsEnabled = true;
                btnEliminarStock.IsEnabled = true;
                btnModificarStock.IsEnabled = true;

                btnEliminarConfirmarStock.Visibility = Visibility.Hidden;
                btnEliminarCancelarStock.Visibility = Visibility.Hidden;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminarCancelarStock_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirStock.IsEnabled = true;
            btnEliminarStock.IsEnabled = true;
            btnModificarStock.IsEnabled = true;

            btnEliminarConfirmarStock.Visibility = Visibility.Hidden;
            btnEliminarCancelarStock.Visibility = Visibility.Hidden;

            tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;
        }

        private void btnModificarStock_Click(object sender, RoutedEventArgs e)
        {
            btnModificarConfirmarStock.Visibility = Visibility.Visible;
            btnModificarCancelarStock.Visibility = Visibility.Visible;

            btnAñadirStock.IsEnabled = false;
            btnEliminarStock.IsEnabled = false;
            btnModificarStock.IsEnabled = false;

            cbxAñadirCategoriaStock.IsHitTestVisible = true;

            tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = false;
        }

        private void btnModificarConfirmarStock_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvStock.SelectedItem as DataRowView;
            string categoria = "";

            try
            {
                DataRow row = tPVDataSet.Tables["Productos"].Rows.Find(selecteditem["id"]);

                row["Nombre"] = tbxNombreStock.Text;
                row["Descripcion"] = tbxDescripcionStock.Text;
                row["Precio"] = Convert.ToDecimal(tbxPrecioStock.Text.Replace(".", ","));
                row["Cantidad"] = Convert.ToInt32(tbxCantidadStock.Text);
                if (cbxAñadirCategoriaStock.SelectedValue != null)
                {
                    row["Categoria"] = cbxAñadirCategoriaStock.SelectedValue;
                    categoria = cbxAñadirCategoriaStock.SelectedValue.ToString();
                }
                else if (cbxAñadirCategoriaStock.Text != "")
                    row["Categoria"] = cbxAñadirCategoriaStock.Text;
                else
                    row["Categoria"] = null;

                tPVDataSetProductosTableAdapter.Update(tPVDataSet);

                btnAñadirStock.IsEnabled = true;
                btnEliminarStock.IsEnabled = true;
                btnModificarStock.IsEnabled = true;

                btnModificarConfirmarStock.Visibility = Visibility.Hidden;
                btnModificarCancelarStock.Visibility = Visibility.Hidden;

                cbxAñadirCategoriaStock.IsHitTestVisible = false;

                tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;

                cargarCategorias(cbxCategoriasStock);
                cargarCategorias(cbxAñadirCategoriaStock);

                if (categoria != "")
                    cbxAñadirCategoriaStock.Text = categoria;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnModificarCancelarStock_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirStock.IsEnabled = true;
            btnEliminarStock.IsEnabled = true;
            btnModificarStock.IsEnabled = true;

            btnModificarConfirmarStock.Visibility = Visibility.Hidden;
            btnModificarCancelarStock.Visibility = Visibility.Hidden;

            cbxCategoriasStock.Text = "";

            cbxAñadirCategoriaStock.IsHitTestVisible = false;

            tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;
        }

        private void btnAñadirCategoriaStock_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion Stock

        #region Proveedores

        private void btnProveedores_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabProveedores.IsSelected = true;
        }

        private void tbxBuscadorProveedores_TextChanged(object sender, TextChangedEventArgs e)
        {
            tPVDataSet.Tables["ClientesVendedores"].DefaultView.RowFilter = "nombre like \'%" + tbxBuscadorClientes.Text + "%\'";
            tPVDataSet.Tables["ClientesVendedores"].DefaultView.RowFilter = "apellidos like \'%" + tbxBuscadorClientes.Text + "%\'";
            tPVDataSet.Tables["ClientesVendedores"].DefaultView.RowFilter = "dni like \'%" + tbxBuscadorClientes.Text + "%\'";
        }

        private void btnAñadirProveedor_Click(object sender, RoutedEventArgs e)
        {
            lvProveedores.SelectedItem = null;

            btnAñadirConfirmarProveedor.Visibility = Visibility.Visible;
            btnAñadirCancelarProveedor.Visibility = Visibility.Visible;

            tbxNombreProveedores.Clear();
            tbxApellidosProveedores.Clear();
            tbxTelefonoProveedores.Clear();
            tbxDireccionProveedores.Clear();
            tbxDNIProveedores.Clear();

            btnAñadirProveedor.IsEnabled = false;
            btnEliminarProveedor.IsEnabled = false;
            btnModificarProveedor.IsEnabled = false;

            tbxCorreoProveedores.IsReadOnly = tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = false;
        }

        private void btnAñadirConfirmarProveedor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = tPVDataSet.Tables["ClientesVendedores"].NewRow();
                row["Nombre"] = tbxNombreProveedores.Text;
                row["Apellidos"] = tbxApellidosProveedores.Text;
                row["Direccion"] = tbxDireccionProveedores.Text;
                row["Telefono"] = Convert.ToInt32(tbxTelefonoProveedores.Text);
                row["Correo"] = tbxCorreoProveedores.Text;
                row["DNI"] = tbxDNIProveedores.Text;

                tPVDataSet.Tables["ClientesVendedores"].Rows.Add(row);

                tPVDataSetClientesVendedoresTableAdapter.Update(tPVDataSet);

                tbxNombreProveedores.Clear();
                tbxApellidosProveedores.Clear();
                tbxDireccionProveedores.Clear();
                tbxTelefonoProveedores.Clear();
                tbxDNIProveedores.Clear();

                btnAñadirProveedor.IsEnabled = true;
                btnEliminarProveedor.IsEnabled = true;
                btnModificarProveedor.IsEnabled = true;

                btnAñadirConfirmarProveedor.Visibility = Visibility.Hidden;
                btnAñadirCancelarProveedor.Visibility = Visibility.Hidden;

                tbxCorreoProveedores.IsReadOnly = tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAñadirCancelarProveedor_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirProveedor.IsEnabled = true;
            btnEliminarProveedor.IsEnabled = true;
            btnModificarProveedor.IsEnabled = true;

            btnAñadirConfirmarProveedor.Visibility = Visibility.Hidden;
            btnAñadirCancelarProveedor.Visibility = Visibility.Hidden;

            tbxCorreoProveedores.IsReadOnly = tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
        }

        private void btnEliminarProveedor_Click(object sender, RoutedEventArgs e)
        {
            btnEliminarConfirmarProveedor.Visibility = Visibility.Visible;
            btnEliminarCancelarProveedor.Visibility = Visibility.Visible;

            btnAñadirProveedor.IsEnabled = false;
            btnEliminarProveedor.IsEnabled = false;
            btnModificarProveedor.IsEnabled = false;
        }

        private void btnEliminarConfirmarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvProveedores.SelectedItem as DataRowView;

            try
            {
                DataRow row = tPVDataSet.Tables["ClientesVendedores"].Rows.Find(selecteditem["id"]);

                row.Delete();

                tPVDataSetClientesVendedoresTableAdapter.Update(tPVDataSet);

                btnAñadirProveedor.IsEnabled = true;
                btnEliminarProveedor.IsEnabled = true;
                btnModificarProveedor.IsEnabled = true;

                btnEliminarConfirmarProveedor.Visibility = Visibility.Hidden;
                btnEliminarCancelarProveedor.Visibility = Visibility.Hidden;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminarCancelarProveedor_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirProveedor.IsEnabled = true;
            btnEliminarProveedor.IsEnabled = true;
            btnModificarProveedor.IsEnabled = true;

            btnEliminarConfirmarProveedor.Visibility = Visibility.Hidden;
            btnEliminarCancelarProveedor.Visibility = Visibility.Hidden;

            tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
        }

        private void btnModificarProveedor_Click(object sender, RoutedEventArgs e)
        {
            btnModificarConfirmarProveedor.Visibility = Visibility.Visible;
            btnModificarCancelarProveedor.Visibility = Visibility.Visible;

            btnAñadirProveedor.IsEnabled = false;
            btnEliminarProveedor.IsEnabled = false;
            btnModificarProveedor.IsEnabled = false;

            tbxCorreoProveedores.IsReadOnly = tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = false;
        }

        private void btnModificarConfirmarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvProveedores.SelectedItem as DataRowView;

            try
            {
                DataRow row = tPVDataSet.Tables["ClientesVendedores"].Rows.Find(selecteditem["id"]);

                row["Nombre"] = tbxNombreProveedores.Text;
                row["Apellidos"] = tbxApellidosProveedores.Text;
                row["Direccion"] = tbxDireccionProveedores.Text;
                row["Telefono"] = Convert.ToInt32(tbxTelefonoProveedores.Text);
                row["Correo"] = tbxTelefonoProveedores.Text;
                row["DNI"] = tbxDNIProveedores.Text;

                tPVDataSetClientesVendedoresTableAdapter.Update(tPVDataSet);

                btnAñadirProveedor.IsEnabled = true;
                btnEliminarProveedor.IsEnabled = true;
                btnModificarProveedor.IsEnabled = true;

                btnModificarConfirmarProveedor.Visibility = Visibility.Hidden;
                btnModificarCancelarProveedor.Visibility = Visibility.Hidden;

                tbxCorreoProveedores.IsReadOnly = tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnModificarCancelarProveedor_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirProveedor.IsEnabled = true;
            btnEliminarProveedor.IsEnabled = true;
            btnModificarProveedor.IsEnabled = true;

            btnModificarConfirmarProveedor.Visibility = Visibility.Hidden;
            btnModificarCancelarProveedor.Visibility = Visibility.Hidden;

            tbxCorreoProveedores.IsReadOnly = tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
        }

        #endregion Proveedores

        #region Clientes

        private void btnClientes_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabClientes.IsSelected = true;
        }

        private void tbxBuscadorClientes_TextChanged(object sender, TextChangedEventArgs e)
        {
            tPVDataSet.Tables["ClientesCompradores"].DefaultView.RowFilter = "nombre like \'%" + tbxBuscadorClientes.Text + "%\'";
            tPVDataSet.Tables["ClientesCompradores"].DefaultView.RowFilter = "apellidos like \'%" + tbxBuscadorClientes.Text + "%\'";
            tPVDataSet.Tables["ClientesCompradores"].DefaultView.RowFilter = "dni like \'%" + tbxBuscadorClientes.Text + "%\'";
        }

        private void btnAñadirCliente_Click(object sender, RoutedEventArgs e)
        {
            lvClientes.SelectedItem = null;

            btnAñadirConfirmarCliente.Visibility = Visibility.Visible;
            btnAñadirCancelarCliente.Visibility = Visibility.Visible;

            tbxNombreClientes.Clear();
            tbxApellidosClientes.Clear();
            tbxTelefonoClientes.Clear();
            tbxDireccionClientes.Clear();
            tbxDNIClientes.Clear();

            btnAñadirCliente.IsEnabled = false;
            btnEliminarCliente.IsEnabled = false;
            btnModificarCliente.IsEnabled = false;

            tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = false;
        }

        private void btnAñadirConfirmarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = tPVDataSet.Tables["ClientesCompradores"].NewRow();
                row["Nombre"] = tbxNombreClientes.Text;
                row["Apellidos"] = tbxApellidosClientes.Text;
                row["Direccion"] = tbxDireccionClientes.Text;
                row["Telefono"] = Convert.ToInt32(tbxTelefonoClientes.Text);
                row["Correo"] = tbxCorreoClientes.Text;
                row["DNI"] = tbxDNIClientes.Text;

                tPVDataSet.Tables["ClientesCompradores"].Rows.Add(row);

                tPVDataSetClientesCompradoresTableAdapter.Update(tPVDataSet);

                tbxNombreClientes.Clear();
                tbxApellidosClientes.Clear();
                tbxDireccionClientes.Clear();
                tbxTelefonoClientes.Clear();
                tbxDNIClientes.Clear();

                btnAñadirCliente.IsEnabled = true;
                btnEliminarCliente.IsEnabled = true;
                btnModificarCliente.IsEnabled = true;

                btnAñadirConfirmarCliente.Visibility = Visibility.Hidden;
                btnAñadirCancelarCliente.Visibility = Visibility.Hidden;

                tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAñadirCancelarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirCliente.IsEnabled = true;
            btnEliminarCliente.IsEnabled = true;
            btnModificarCliente.IsEnabled = true;

            btnAñadirConfirmarCliente.Visibility = Visibility.Hidden;
            btnAñadirCancelarCliente.Visibility = Visibility.Hidden;

            tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
        }

        private void btnEliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnEliminarConfirmarCliente.Visibility = Visibility.Visible;
            btnEliminarCancelarCliente.Visibility = Visibility.Visible;

            btnAñadirCliente.IsEnabled = false;
            btnEliminarCliente.IsEnabled = false;
            btnModificarCliente.IsEnabled = false;
        }

        private void btnEliminarConfirmarCliente_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvClientes.SelectedItem as DataRowView;

            try
            {
                DataRow row = tPVDataSet.Tables["ClientesCompradores"].Rows.Find(selecteditem["id"]);

                row.Delete();

                tPVDataSetClientesCompradoresTableAdapter.Update(tPVDataSet);

                btnAñadirCliente.IsEnabled = true;
                btnEliminarCliente.IsEnabled = true;
                btnModificarCliente.IsEnabled = true;

                btnEliminarConfirmarCliente.Visibility = Visibility.Hidden;
                btnEliminarCancelarCliente.Visibility = Visibility.Hidden;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminarCancelarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirCliente.IsEnabled = true;
            btnEliminarCliente.IsEnabled = true;
            btnModificarCliente.IsEnabled = true;

            btnEliminarConfirmarCliente.Visibility = Visibility.Hidden;
            btnEliminarCancelarCliente.Visibility = Visibility.Hidden;

            tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
        }

        private void btnModificarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnModificarConfirmarCliente.Visibility = Visibility.Visible;
            btnModificarCancelarCliente.Visibility = Visibility.Visible;

            btnAñadirCliente.IsEnabled = false;
            btnEliminarCliente.IsEnabled = false;
            btnModificarCliente.IsEnabled = false;

            tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = false;
        }

        private void btnModificarConfirmarCliente_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvClientes.SelectedItem as DataRowView;

            try
            {
                DataRow row = tPVDataSet.Tables["ClientesCompradores"].Rows.Find(selecteditem["id"]);

                row["Nombre"] = tbxNombreClientes.Text;
                row["Apellidos"] = tbxApellidosClientes.Text;
                row["Direccion"] = tbxDireccionClientes.Text;
                row["Telefono"] = Convert.ToInt32(tbxTelefonoClientes.Text);
                row["Correo"] = tbxCorreoClientes.Text;
                row["DNI"] = tbxDNIClientes.Text;

                tPVDataSetClientesCompradoresTableAdapter.Update(tPVDataSet);

                btnAñadirCliente.IsEnabled = true;
                btnEliminarCliente.IsEnabled = true;
                btnModificarCliente.IsEnabled = true;

                btnModificarConfirmarCliente.Visibility = Visibility.Hidden;
                btnModificarCancelarCliente.Visibility = Visibility.Hidden;

                tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnModificarCancelarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnAñadirCliente.IsEnabled = true;
            btnEliminarCliente.IsEnabled = true;
            btnModificarCliente.IsEnabled = true;

            btnModificarConfirmarCliente.Visibility = Visibility.Hidden;
            btnModificarCancelarCliente.Visibility = Visibility.Hidden;

            tbxCorreoClientes.IsReadOnly = tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
        }

        #endregion Clientes

        #region Vender

        private void btnVender_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Cargo las categorias de los productos
            cargarCategorias(cbxCategoriasVender);

            tabControl.Visibility = Visibility.Visible;
            tabVender.IsSelected = true;
        }

        private void cbxCategoriasVender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategoria = cbxCategoriasVender.SelectedItem as DataRowView;

            if (selectedCategoria != null)
                tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "Categoria like \'%" + selectedCategoria["Categoria"] + "%\'";
            else
                tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "";
        }

        private void tbxBuscadorVender_TextChanged(object sender, TextChangedEventArgs e)
        {
            tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "nombre like \'%" + tbxBuscadorVender.Text + "%\'";
        }

        private void btnAñadirProductoVenta_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var productoSeleccionado = lvVender.SelectedItem as DataRowView;
            var clienteSeleccionado = cbxClienteVentas.SelectedItem as DataRowView;

            if (tbxCantidadVender.Text.Length > 0)
            {

                try
                {
                    // Recupero los datos del producto seleccionado
                    DataRow producto = tPVDataSet.Tables["Productos"].Rows.Find(productoSeleccionado["id"]);

                    decimal precio = Convert.ToDecimal(producto["precio"]);
                    int cantidad = Convert.ToInt32(tbxCantidadVender.Text);

                    // Compruebo que haya suficientes unidades de ese producto
                    if (Convert.ToInt32(producto["cantidad"]) < cantidad)
                    {
                        throw new InvalidOperationException("No hay suficientes unidades del producto seleccionado para realizar la venta.");
                    }
                    else
                    {
                        producto["cantidad"] = Convert.ToInt32(producto["cantidad"]) - cantidad;
                    }

                    // Creo la nueva fila del resumen de venta y la añado al datagrid
                    DataRow r = dataTableResumenVenta.NewRow();
                    r["Producto"] = producto["nombre"];
                    r["Precio"] = producto["precio"];
                    r["Cantidad"] = tbxCantidadVender.Text;
                    r["Total"] = precio * cantidad;

                    dataTableResumenVenta.Rows.Add(r);

                    // Creo una nueva linea de venta
                    DataRow lineaVenta = tPVDataSet.Tables["LineasVentas"].NewRow();

                    //Si es una venta nueva creo una cabecera de venta
                    if (numProductosResumenVenta == 0)
                    {
                        DataRow cabeceraVenta = tPVDataSet.Tables["CabecerasVentas"].NewRow();
                        if (cbxClienteVentas.SelectedItem != null)
                            cabeceraVenta["idCliente"] = clienteSeleccionado["id"];
                        cabeceraVenta["fecha"] = DateTime.Now;
                        idCabeceraVenta = Convert.ToInt32(cabeceraVenta["id"]);
                        tPVDataSet.Tables["CabecerasVentas"].Rows.Add(cabeceraVenta);
                    }

                    // Guardo los datos de la linea de venta
                    lineaVenta["idCabecera"] = idCabeceraVenta;
                    lineaVenta["idProducto"] = productoSeleccionado["id"];
                    lineaVenta["Cantidad"] = Convert.ToInt32(tbxCantidadVender.Text);
                    lineaVenta["PrecioTotal"] = precio * cantidad;

                    numProductosResumenVenta++;

                    tPVDataSet.Tables["LineasVentas"].Rows.Add(lineaVenta);

                    tbxTotalVenta.Text = dataTableResumenVenta.Compute("SUM(Total)", String.Empty).ToString();

                    //Habilitar el botón de terminar venta
                    btnTerminarVenta.IsEnabled = true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("Debe seleccionar una cantidad antes de poder añadir un producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTerminarVenta_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (cbxClienteVentas.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.", "Completar venta", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else if (MessageBox.Show("¿Está seguro de que quiere completar la venta?", "Completar venta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                tPVDataSetCabecerasVentasTableAdapter.Update(tPVDataSet);
                tPVDataSetLineasVentasTableAdapter.Update(tPVDataSet);
                tPVDataSetProductosTableAdapter.Update(tPVDataSet);
            }
        }

        private void resumenVentaRow_Deleted(object sender, DataRowChangeEventArgs e)
        {
            numProductosResumenVenta--;

            if (numProductosResumenVenta == 0)
            {
                tPVDataSet.Tables["CabecerasVentas"].Rows.Find(idCabeceraVenta).Delete();
                btnTerminarVenta.IsEnabled = false;
                cbxClienteVentas.SelectedItem = null;
            }

            tbxTotalVenta.Text = dataTableResumenVenta.Compute("SUM(Total)", String.Empty).ToString();
        }
        #endregion        

        #region Comprar

        private void btnComprar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Cargo las categorias de los productos
            cargarCategorias(cbxCategoriasComprar);

            tabControl.Visibility = Visibility.Visible;
            tabComprar.IsSelected = true;
        }

        private void tbxBuscadorComprar_TextChanged(object sender, TextChangedEventArgs e)
        {
            tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "nombre like \'%" + tbxBuscadorComprar.Text + "%\'";
        }

        private void cbxCategoriasComprar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategoria = cbxCategoriasComprar.SelectedItem as DataRowView;

            if (selectedCategoria != null)
                tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "Categoria like \'%" + selectedCategoria["Categoria"] + "%\'";
            else
                tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "";
        }

        private void tbxNuevoProductoCompra_TextChanged(object sender, TextChangedEventArgs e)
        {
            lvComprar.SelectedItem = null;

            if (tbxNuevoProductoCompra.Text.Length == 0)
                tbxPrecioComprar.IsEnabled = false;
            else tbxPrecioComprar.IsEnabled = true;
        }

        private void btnAñadirProductoCompra_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var productoSeleccionado = lvComprar.SelectedItem as DataRowView;
            int idProductoAñadido;
            var clienteSeleccionado = cbxClienteCompras.SelectedItem as DataRowView;
            decimal precio;
            int cantidad;

            if (tbxCantidadComprar.Text.Length > 0)
            {
                try
                {
                    // Si se selecciona un producto ya existente
                    if (productoSeleccionado != null)
                    {
                        DataRow producto = tPVDataSet.Tables["Productos"].Rows.Find(productoSeleccionado["id"]);

                        idProductoAñadido = Convert.ToInt32(productoSeleccionado["id"]);

                        precio = Convert.ToDecimal(producto["precio"]);
                        cantidad = Convert.ToInt32(tbxCantidadComprar.Text);

                        producto["cantidad"] = Convert.ToInt32(producto["cantidad"]) + cantidad;

                        DataRow productoAñadido = dataTableResumenCompra.NewRow();
                        productoAñadido["Producto"] = producto["nombre"];
                        productoAñadido["Precio"] = producto["precio"];
                        productoAñadido["Cantidad"] = tbxCantidadComprar.Text;
                        productoAñadido["Total"] = precio * cantidad;

                        dataTableResumenCompra.Rows.Add(productoAñadido);
                    }
                    // Si se decide añadir un producto nuevo
                    else
                    {
                        DataRow nuevoProducto = tPVDataSet.Tables["Productos"].NewRow();

                        idProductoAñadido = Convert.ToInt32(nuevoProducto["id"]);

                        nuevoProducto["Nombre"] = tbxNuevoProductoCompra.Text;
                        nuevoProducto["Cantidad"] = Convert.ToInt32(tbxCantidadComprar.Text);
                        nuevoProducto["Precio"] = Convert.ToDecimal(tbxPrecioComprar.Text.Replace(".", ","));

                        tPVDataSet.Tables["Productos"].Rows.Add(nuevoProducto);

                        precio = Convert.ToDecimal(tbxPrecioComprar.Text);
                        cantidad = Convert.ToInt32(tbxCantidadComprar.Text);

                        DataRow productoAñadido = dataTableResumenCompra.NewRow();
                        productoAñadido["Producto"] = nuevoProducto["nombre"];
                        productoAñadido["Precio"] = nuevoProducto["precio"];
                        productoAñadido["Cantidad"] = tbxCantidadComprar.Text;
                        productoAñadido["Total"] = precio * cantidad;

                        dataTableResumenCompra.Rows.Add(productoAñadido);
                    }

                    DataRow lineaCompra = tPVDataSet.Tables["LineasCompras"].NewRow();

                    if (numProductosResumenCompra == 0)
                    {
                        DataRow cabeceraCompra = tPVDataSet.Tables["CabecerasCompras"].NewRow();
                        if (cbxClienteCompras.SelectedItem != null)
                            cabeceraCompra["idCliente"] = clienteSeleccionado["id"];
                        cabeceraCompra["fecha"] = DateTime.Now;
                        idCabeceraCompra = Convert.ToInt32(cabeceraCompra["id"]);
                        tPVDataSet.Tables["CabecerasCompras"].Rows.Add(cabeceraCompra);
                    }

                    lineaCompra["idCabecera"] = idCabeceraCompra;
                    lineaCompra["idProducto"] = idProductoAñadido;
                    lineaCompra["Cantidad"] = Convert.ToInt32(tbxCantidadComprar.Text);
                    lineaCompra["PrecioTotal"] = precio * cantidad;

                    numProductosResumenCompra++;

                    tPVDataSet.Tables["LineasCompras"].Rows.Add(lineaCompra);

                    tbxTotalCompra.Text = dataTableResumenCompra.Compute("SUM(Total)", String.Empty).ToString();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una cantidad antes de poder añadir un producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTerminarCompra_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (cbxClienteCompras.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.", "Completar venta", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else if (MessageBox.Show("¿Está seguro de que quiere completar la venta?", "Completar venta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                tPVDataSetCabecerasComprasTableAdapter.Update(tPVDataSet);
                tPVDataSetLineasComprasTableAdapter.Update(tPVDataSet);
                tPVDataSetProductosTableAdapter.Update(tPVDataSet);
            }
        }

        private void resumenCompraRow_Deleted(object sender, DataRowChangeEventArgs e)
        {
            numProductosResumenCompra--;

            if (numProductosResumenCompra == 0)
            {
                tPVDataSet.Tables["CabecerasCompras"].Rows.Find(idCabeceraCompra).Delete();
                btnTerminarCompra.IsEnabled = false;
                cbxClienteCompras.SelectedItem = null;
            }
            tbxTotalCompra.Text = dataTableResumenCompra.Compute("SUM(Total)", String.Empty).ToString();
        }

        #endregion

        #region Menu

        private void btnExportarComprasExcel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            DataRow[] lineasCompra;
            DataRow cabeceraCompra;
            DataRow producto;
            DataRow cliente;
            string[] datos;
            int filasExcel = 1;
            string rutaGuardado = "";


            try
            {

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

                fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    rutaGuardado = fbd.SelectedPath + "\\";
                }


                for (int i = 0; i < tPVDataSet.Tables["CabecerasCompras"].Rows.Count; i++)
                {
                    cabeceraCompra = tPVDataSet.Tables["CabecerasCompras"].Rows[i];
                    lineasCompra = tPVDataSet.Tables["CabecerasCompras"].Rows[i].GetChildRows("FK_CabecerasCompras_LineasCompras");
                    for (int j = 0; j < lineasCompra.Length; j++)
                    {
                        producto = tPVDataSet.Tables["Productos"].Rows.Find(lineasCompra[j]["idProducto"]);
                        cliente = tPVDataSet.Tables["ClientesVendedores"].Rows.Find(cabeceraCompra["idCliente"]);

                        datos = new string[] { cabeceraCompra["fecha"].ToString(), cliente["nombre"] + " " + cliente["apellidos"], cliente["DNI"].ToString(), producto["nombre"].ToString(), lineasCompra[j]["PrecioTotal"].ToString() };
                        for (int k = 0; k < datos.Length; k++)
                        {
                            xlWorkSheet.Cells[filasExcel, k + 1] = datos[k];
                        }
                        filasExcel++;
                    }
                }

                xlWorkBook.SaveAs(rutaGuardado + "compras.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                MessageBox.Show("Archivo Excel creado, puedes encontrarlo en " + rutaGuardado);
            }
            catch (System.Runtime.InteropServices.COMException exception)
            {
                if (exception.Message != "Excepción de HRESULT: 0x800A03EC")
                {
                    MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExportarVentasExcel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            DataRow[] lineasVenta;
            DataRow cabeceraVenta;
            DataRow producto;
            DataRow cliente;
            string[] datos;
            int filasExcel = 1;
            string rutaGuardado = "";


            try
            {
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

                fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    rutaGuardado = fbd.SelectedPath + "\\";
                }


                for (int i = 0; i < tPVDataSet.Tables["CabecerasVentas"].Rows.Count; i++)
                {
                    cabeceraVenta = tPVDataSet.Tables["CabecerasVentas"].Rows[i];
                    lineasVenta = tPVDataSet.Tables["CabecerasVentas"].Rows[i].GetChildRows("FK_CabecerasVentas_LineasVentas");
                    for (int j = 0; j < lineasVenta.Length; j++)
                    {
                        producto = tPVDataSet.Tables["Productos"].Rows.Find(lineasVenta[j]["idProducto"]);
                        cliente = tPVDataSet.Tables["ClientesCompradores"].Rows.Find(cabeceraVenta["idCliente"]);

                        datos = new string[] { cabeceraVenta["fecha"].ToString(), cliente["nombre"] + " " + cliente["apellidos"], cliente["DNI"].ToString(), producto["nombre"].ToString(), lineasVenta[j]["PrecioTotal"].ToString() };
                        for (int k = 0; k < datos.Length; k++)
                        {
                            xlWorkSheet.Cells[filasExcel, k + 1] = datos[k];
                        }
                        filasExcel++;
                    }
                }

                xlWorkBook.SaveAs(rutaGuardado + "Ventas.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                MessageBox.Show("Archivo Excel creado, puedes encontrarlo en " + rutaGuardado);
            }
            catch (System.Runtime.InteropServices.COMException exception)
            {
                if (exception.Message != "Excepción de HRESULT: 0x800A03EC")
                {
                    MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion

        private void cargarCategorias(ComboBox cbx)
        {
            DataView view = new DataView(tPVDataSet.Tables["Productos"]);
            view.Sort = "Categoria";
            DataTable distinctCategorias = view.ToTable(true, "Categoria");

            cbx.ItemsSource = distinctCategorias.DefaultView;
            cbx.SelectedValuePath = "Categoria";
            cbx.DisplayMemberPath = "Categoria";
        }

        private DataTable crearDataTableResumen(string nombre)
        {
            DataTable dataTableResumen = new DataTable(nombre);
            dataTableResumen.Columns.Add("Producto");
            dataTableResumen.Columns.Add("Precio");
            dataTableResumen.Columns["Precio"].DataType = System.Type.GetType("System.Decimal");
            dataTableResumen.Columns.Add("Cantidad");
            dataTableResumen.Columns["Cantidad"].DataType = System.Type.GetType("System.Int32");
            dataTableResumen.Columns.Add("Total");
            dataTableResumen.Columns["Total"].DataType = System.Type.GetType("System.Decimal");

            dataTableResumen.Columns["Producto"].ReadOnly = true;
            dataTableResumen.Columns["Total"].ReadOnly = true;

            dataTableResumen.Columns["Total"].Expression = "(Precio * Cantidad)";

            return dataTableResumen;
        }

        public static bool onlyInteger(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }
        public static bool onlyDecimal(string text)
        {
            Regex regex = new Regex("[^0-9.,]+");
            return !regex.IsMatch(text);
        }

        private void tbxInteger_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !onlyInteger(e.Text);
        }

        private void tbxDecimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !onlyInteger(e.Text);
        }

        private void btnMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (btnExportarComprasExcel.Visibility == Visibility.Hidden)
                btnExportarVentasExcel.Visibility = btnExportarComprasExcel.Visibility = Visibility.Visible;
            else
                btnExportarVentasExcel.Visibility = btnExportarComprasExcel.Visibility = Visibility.Hidden;
        }
    }
}