<%@ Page Title="Páginas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Paginas.aspx.cs" Inherits="GenericaWeb.Admin.Paginas" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Páginas</h2>
    <p class="text-muted">Registre aquí cada página .aspx protegida. La ruta debe coincidir con la ruta relativa de la aplicación (por ejemplo <code>~/Admin/Usuarios.aspx</code>).</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-7">
            <asp:GridView ID="gvPaginas" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvPaginas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="PaginaId" HeaderText="Id" />
                    <asp:BoundField DataField="PaginaDesc" HeaderText="Ruta" />
                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                    <asp:BoundField DataField="PaginaUsys" HeaderText="Modificó" />
                    <asp:BoundField DataField="PaginaDT" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("PaginaId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("PaginaId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar la página y sus permisos?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div class="col-lg-5">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nueva página" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-2">
                    <label class="form-label">Ruta (PaginaDesc)</label>
                    <asp:TextBox ID="txtRuta" runat="server" CssClass="form-control" MaxLength="50" placeholder="~/Carpeta/Pagina.aspx" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRuta" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtRuta" ValidationExpression="^~/[^?#]+$"
                                                    ErrorMessage="Debe iniciar con ~/ y no incluir parámetros" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Descripción (texto del menú)</label>
                    <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" MaxLength="150" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescripcion" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
