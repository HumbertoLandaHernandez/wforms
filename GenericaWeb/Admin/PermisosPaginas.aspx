<%@ Page Title="Permisos por página" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PermisosPaginas.aspx.cs" Inherits="GenericaWeb.Admin.PermisosPaginas" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Permisos por página</h2>
    <p class="text-muted">Una lista agrupa las páginas permitidas (<code>Sistema.PermisoLista</code>). Se asigna a roles (<em>RolPermisosId</em>)
        y, de forma adicional, a usuarios (<em>UsuarioPermisosPaginasId</em>).</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-5">
            <asp:GridView ID="gvListas" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvListas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="PermisoListaId" HeaderText="Lista" />
                    <asp:BoundField DataField="Paginas" HeaderText="Páginas" />
                    <asp:BoundField DataField="Roles" HeaderText="Roles" />
                    <asp:BoundField DataField="Usuarios" HeaderText="Usuarios" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("PermisoListaId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("PermisoListaId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar la lista? Los roles y usuarios que la usan perderán esos permisos.');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay listas de permisos.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-7">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <div class="row g-2 align-items-end mb-3">
                    <div class="col-sm-6">
                        <label class="form-label">Número de lista (PermisoListaId)</label>
                        <asp:TextBox ID="txtListaId" runat="server" CssClass="form-control" TextMode="Number" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtListaId" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                        <asp:RangeValidator runat="server" ControlToValidate="txtListaId" Type="Integer" MinimumValue="1" MaximumValue="2147483647"
                                            ErrorMessage="Número entero mayor a 0" CssClass="text-danger small" Display="Dynamic" />
                    </div>
                    <div class="col-sm-6">
                        <asp:Button ID="btnNuevo" runat="server" Text="Nueva lista" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                    </div>
                </div>
                <label class="form-label">Páginas permitidas</label>
                <div class="border rounded p-2 mb-3 lista-paginas">
                    <asp:CheckBoxList ID="cblPaginas" runat="server" RepeatLayout="Flow" RepeatDirection="Vertical" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
