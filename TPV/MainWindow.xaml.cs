using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TPV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Numero de productos añadidos a la ventana de venta
        int numProductosResumen;
        // Productos disponibles para añadir descuento
        string[] productosDescuentos;

        private TPV.TPVDataSetTableAdapters.ProductosTableAdapter tPVDataSetProductosTableAdapter;
        private TPV.TPVDataSetTableAdapters.ClientesCompradoresTableAdapter tPVDataSetClientesCompradoresTableAdapter;
        private TPV.TPVDataSetTableAdapters.ClientesVendedoresTableAdapter tPVDataSetClientesVendedoresTableAdapter;
        private TPV.TPVDataSetTableAdapters.LineasVentasTableAdapter tPVDataSetLineasVentasTableAdapter;

        private TPV.TPVDataSet tPVDataSet;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControl.Visibility = Visibility.Hidden;

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
            // Load data into the table LineasVentas. You can modify this code as needed.
            tPVDataSetLineasVentasTableAdapter = new TPV.TPVDataSetTableAdapters.LineasVentasTableAdapter();
            tPVDataSetLineasVentasTableAdapter.Fill(tPVDataSet.LineasVentas);
            System.Windows.Data.CollectionViewSource lineasVentasViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("lineasVentasViewSource")));
            lineasVentasViewSource.View.MoveCurrentToFirst();
        }

        #region Stock

        private void btnStock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabStock.IsSelected = true;
        }

        private void btnAñadirStock_Click(object sender, RoutedEventArgs e)
        {
            cbxStock.SelectedItem = null;
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

            tbxCantidadStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxNombreStock.IsReadOnly = tbxPrecioStock.IsReadOnly = false;
        }

        private void btnAñadirConfirmarStock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow row = tPVDataSet.Tables["Productos"].NewRow();
                row["Nombre"] = tbxNombreStock.Text;
                row["Descripcion"] = tbxDescripcionStock.Text;
                //row["idCategoria"] = cbxCategoriaStock.SelectedValue.ToString();
                row["Precio"] = Convert.ToDecimal(tbxPrecioStock.Text);
                row["Cantidad"] = Convert.ToInt32(tbxCantidadStock.Text);

                tPVDataSet.Tables["Productos"].Rows.Add(row);

                tPVDataSetProductosTableAdapter.Update(tPVDataSet);

                tbxNombreStock.Clear();
                tbxDescripcionStock.Clear();
                tbxPrecioStock.Clear();
                tbxCantidadStock.Clear();

                btnAñadirStock.IsEnabled = true;
                btnEliminarStock.IsEnabled = true;
                btnModificarStock.IsEnabled = true;

                btnAñadirConfirmarStock.Visibility = Visibility.Hidden;
                btnAñadirCancelarStock.Visibility = Visibility.Hidden;

                tbxCantidadStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxNombreStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;
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

            tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = false;
        }

        private void btnModificarConfirmarStock_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lvStock.SelectedItem as DataRowView;

            try
            {
                DataRow row = tPVDataSet.Tables["Productos"].Rows.Find(selecteditem["id"]);

                row["Nombre"] = tbxNombreStock.Text;
                row["Descripcion"] = tbxDescripcionStock.Text;
                row["Precio"] = Convert.ToDecimal(tbxPrecioStock.Text);
                row["Cantidad"] = Convert.ToInt32(tbxCantidadStock.Text);
                //row["idCategoria"] = cbxCategoriaStock.SelectedValue.ToString();

                tPVDataSetProductosTableAdapter.Update(tPVDataSet);

                btnAñadirStock.IsEnabled = true;
                btnEliminarStock.IsEnabled = true;
                btnModificarStock.IsEnabled = true;

                btnModificarConfirmarStock.Visibility = Visibility.Hidden;
                btnModificarCancelarStock.Visibility = Visibility.Hidden;

                tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;
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

            tbxNombreStock.IsReadOnly = tbxDescripcionStock.IsReadOnly = tbxCantidadStock.IsReadOnly = tbxPrecioStock.IsReadOnly = true;
        }

        private void btnAñadirCategoriaStock_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cbxCategoriaStock_TextChanged(object sender, RoutedEventArgs e)
        {
            btnAñadirCategoriaStock.IsEnabled = true;
        }

        #endregion Stock

        #region Proveedores

        private void btnProveedores_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabProveedores.IsSelected = true;
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

            tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = false;
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

                tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
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

            tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
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

            tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = false;
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
                row["DNI"] = tbxDNIProveedores.Text;

                tPVDataSetClientesVendedoresTableAdapter.Update(tPVDataSet);

                btnAñadirProveedor.IsEnabled = true;
                btnEliminarProveedor.IsEnabled = true;
                btnModificarProveedor.IsEnabled = true;

                btnModificarConfirmarProveedor.Visibility = Visibility.Hidden;
                btnModificarCancelarProveedor.Visibility = Visibility.Hidden;

                tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
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

            tbxNombreProveedores.IsReadOnly = tbxApellidosProveedores.IsReadOnly = tbxDireccionProveedores.IsReadOnly = tbxTelefonoProveedores.IsReadOnly = tbxDNIProveedores.IsReadOnly = true;
        }

        #endregion Proveedores

        #region Clientes

        private void btnClientes_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabClientes.IsSelected = true;
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

            tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = false;
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

                tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
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

            tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
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
        }

        private void btnModificarCliente_Click(object sender, RoutedEventArgs e)
        {
            btnModificarConfirmarCliente.Visibility = Visibility.Visible;
            btnModificarCancelarCliente.Visibility = Visibility.Visible;

            btnAñadirCliente.IsEnabled = false;
            btnEliminarCliente.IsEnabled = false;
            btnModificarCliente.IsEnabled = false;

            tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = false;
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
                row["DNI"] = tbxDNIClientes.Text;

                tPVDataSetClientesCompradoresTableAdapter.Update(tPVDataSet);

                btnAñadirCliente.IsEnabled = true;
                btnEliminarCliente.IsEnabled = true;
                btnModificarCliente.IsEnabled = true;

                btnModificarConfirmarCliente.Visibility = Visibility.Hidden;
                btnModificarCancelarCliente.Visibility = Visibility.Hidden;

                tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
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

            tbxNombreClientes.IsReadOnly = tbxApellidosClientes.IsReadOnly = tbxDireccionClientes.IsReadOnly = tbxTelefonoClientes.IsReadOnly = tbxDNIClientes.IsReadOnly = true;
        }

        #endregion Clientes

        #region Vender

        private void btnVender_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Visibility = Visibility.Visible;
            tabVender.IsSelected = true;
        }

        private void tbxBuscadorVender_TextChanged(object sender, TextChangedEventArgs e)
        {
            tPVDataSet.Tables["Productos"].DefaultView.RowFilter = "nombre like \'%" + tbxBuscadorVender.Text + "%\'";
        }

        private void btnAñadirProductoVenta_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = lvVender.SelectedItem as DataRowView;

            try
            {
                DataRow cabecera = tPVDataSet.Tables["Productos"].NewRow();
                DataRow linea = tPVDataSet.Tables["Productos"].NewRow();

                DataRow row = tPVDataSet.Tables["Productos"].Rows.Find(selectedItem["id"]);

                numProductosResumen++;

                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(30);

                resumenVenta.RowDefinitions.Add(r);

                //Nombre del producto
                AddLabelToGridResumenVentas(row[1].ToString(), numProductosResumen, 0);

                AddSplitterToGridResumenVentas(numProductosResumen, 1);

                //Precio
                AddLabelToGridResumenVentas(row[4].ToString(), numProductosResumen, 2);
                decimal precio = Convert.ToDecimal(row[4]);

                AddSplitterToGridResumenVentas(numProductosResumen, 3);

                //Cantidad
                AddLabelToGridResumenVentas(tbxCantidadStock.Text, numProductosResumen, 4);
                int cantidad = Convert.ToInt32(tbxCantidadStock.Text);

                AddSplitterToGridResumenVentas(numProductosResumen, 5);

                //Total
                string total =(precio * cantidad).ToString();
                AddLabelToGridResumenVentas(total, numProductosResumen, 6);

                AddSplitterToGridResumenVentas(numProductosResumen, 7);

                tbxCantidadVender.Clear();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }     
        #endregion

        public void AddLabelToGridResumenVentas(String content, int row, int column)
        {
            Label label = new Label();
            label.Content = content;

            label.SetValue(Grid.RowProperty, row);
            label.SetValue(Grid.ColumnProperty, column);

            resumenVenta.Children.Add(label);
        }

        public void AddSplitterToGridResumenVentas(int row, int column)
        {
            GridSplitter splitter = new GridSplitter();

            splitter.SetValue(Grid.RowProperty, row);
            splitter.SetValue(Grid.ColumnProperty, column);

            splitter.Width = 1;

            splitter.HorizontalAlignment = HorizontalAlignment.Center;

            resumenVenta.Children.Add(splitter);
        }


    }
}