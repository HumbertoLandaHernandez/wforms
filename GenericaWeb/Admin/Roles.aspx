<%@ Page Title="Roles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Roles.aspx.cs" Inherits="GenericaWeb.Admin.Roles" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Roles</h2>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-7">
            <asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvRoles_RowCommand">
                <Columns>
                    <asp:BoundField DataField="RolId" HeaderText="Id" />
                    <asp:BoundField DataField="RolDesc" HeaderText="Rol" />
                    <asp:BoundField DataField="PaginaHome" HeaderText="Página de inicio" />
                    <asp:BoundField DataField="RolPermisosId" HeaderText="Lista páginas" />
                    <asp:BoundField DataField="RolClassId" HeaderText="Clase" />
                    <asp:BoundField DataField="RolUsys" HeaderText="Modificó" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("RolId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("RolId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar el rol?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay roles registrados.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-5">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nuevo rol" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-2">
                    <label class="form-label">Descripción</label>
                    <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDesc" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Página de inicio <small class="text-muted">(Id ≤ 255)</small></label>
                    <asp:DropDownList ID="ddlPaginaHome" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlPaginaHome" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Lista de páginas permitidas</label>
                    <asp:DropDownList ID="ddlPermisos" runat="server" CssClass="form-select" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlPermisos" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Clase (RolClassId)</label>
                    <asp:TextBox ID="txtClassId" runat="server" CssClass="form-control" TextMode="Number" Text="0" />
                    <asp:RangeValidator runat="server" ControlToValidate="txtClassId" Type="Integer" MinimumValue="0" MaximumValue="255"
                                        ErrorMessage="Valor entre 0 y 255" CssClass="text-danger small" Display="Dynamic" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtClassId" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
