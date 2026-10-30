<%@ Page Title="Menú" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListasPaginas.aspx.cs" Inherits="GenericaWeb.Admin.ListasPaginas" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Menú (listas de páginas)</h2>
    <p class="text-muted">Cada lista (<code>Sistema.ListaPagina</code>) es un grupo del menú; sus elementos
        (<code>Sistema.ListaPaginaElemento</code>) son las páginas que muestra. Cada usuario ve solo las páginas a las que tiene permiso.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-6">
            <asp:GridView ID="gvListas" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvListas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ListaPaginaId" HeaderText="Id" />
                    <asp:BoundField DataField="ListaPaginaDesc" HeaderText="Grupo" />
                    <asp:BoundField DataField="ListaPaginaComentario" HeaderText="Comentario" />
                    <asp:BoundField DataField="Paginas" HeaderText="Páginas" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("ListaPaginaId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("ListaPaginaId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar el grupo del menú?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay grupos de menú.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-6">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nuevo grupo" /></h5>
                <asp:HiddenField ID="hfId" runat="server" />
                <div class="mb-2">
                    <label class="form-label">Descripción</label>
                    <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDesc" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="mb-2">
                    <label class="form-label">Comentario</label>
                    <asp:TextBox ID="txtComentario" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500" />
                </div>
                <label class="form-label">Páginas del grupo</label>
                <div class="border rounded p-2 mb-3 lista-paginas">
                    <asp:CheckBoxList ID="cblPaginas" runat="server" RepeatLayout="Flow" RepeatDirection="Vertical" />
                </div>
                <div>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" CssClass="btn btn-secondary" OnClick="btnNuevo_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
