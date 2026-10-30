<%@ Page Title="Composición de nombres" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NombresLista.aspx.cs" Inherits="GenericaWeb.Admin.NombresLista" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Composición de nombres</h2>
    <p class="text-muted">Relaciona cada nombre personal con sus partes (<code>Sistema.NombreLista</code>): tipo de nombre + nombre base.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body mb-4" DefaultButton="btnAgregar">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label">Nombre personal</label>
                <asp:DropDownList ID="ddlNombrePersonal" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNombrePersonal" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Tipo</label>
                <asp:DropDownList ID="ddlNombreTipo" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNombreTipo" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Nombre base</label>
                <asp:DropDownList ID="ddlNombreBase" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlNombreBase" InitialValue="" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnAgregar" runat="server" Text="Agregar" CssClass="btn btn-primary w-100" OnClick="btnAgregar_Click" />
            </div>
        </div>
    </asp:Panel>
    <asp:GridView ID="gvNombres" runat="server" AutoGenerateColumns="false" GridLines="None"
                  CssClass="table table-sm table-striped table-hover" OnRowCommand="gvNombres_RowCommand">
        <Columns>
            <asp:BoundField DataField="NombrePersonalDesc" HeaderText="Nombre personal" />
            <asp:BoundField DataField="NombreTipoDesc" HeaderText="Tipo" />
            <asp:BoundField DataField="NombreBaseDesc" HeaderText="Nombre base" />
            <asp:BoundField DataField="NombreListaUsys" HeaderText="Modificó" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" CommandName="Eliminar"
                                    CommandArgument='<%# Eval("NombrePersonalId") + "|" + Eval("NombreTipoId") + "|" + Eval("NombreBaseId") %>'
                                    CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                    OnClientClick="return confirm('¿Eliminar la relación?');">Eliminar</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>Sin registros.</EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
