<%@ Page Title="Listas de páginas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListasPaginas.aspx.cs" Inherits="GenericaWeb.Admin.ListasPaginas" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mb-3">Listas de páginas (menú)</h2>
    <p class="text-muted">Cada lista (<code>Sistema.PaginaLista</code>) agrupa páginas (<code>Sistema.PaginaListaElemento</code>).
        Se asigna a cada usuario como sus páginas permitidas (<em>UsuarioPermisosPaginasId</em>) y también forma los grupos del menú;
        cada usuario ve en el menú solo las páginas a las que tiene acceso.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" EnableViewState="false" />
    <div class="row g-4">
        <div class="col-lg-6">
            <asp:GridView ID="gvListas" runat="server" AutoGenerateColumns="false" GridLines="None"
                          CssClass="table table-sm table-striped table-hover" OnRowCommand="gvListas_RowCommand">
                <Columns>
                    <asp:BoundField DataField="PaginaListaId" HeaderText="Id" />
                    <asp:BoundField DataField="PaginaListaDesc" HeaderText="Lista" />
                    <asp:BoundField DataField="PaginaListaComentario" HeaderText="Comentario" />
                    <asp:BoundField DataField="Paginas" HeaderText="Páginas" />
                    <asp:BoundField DataField="Usuarios" HeaderText="Usuarios" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("PaginaListaId") %>'
                                            CssClass="btn btn-sm btn-outline-primary" CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("PaginaListaId") %>'
                                            CssClass="btn btn-sm btn-outline-danger" CausesValidation="false" Visible='<%# PuedeEditar %>'
                                            OnClientClick="return confirm('¿Eliminar la lista de páginas?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>No hay listas de páginas.</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="col-lg-6">
            <asp:Panel ID="pnlEdicion" runat="server" CssClass="card card-body" DefaultButton="btnGuardar">
                <h5><asp:Literal ID="litTitulo" runat="server" Text="Nueva lista" /></h5>
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
                <label class="form-label">Páginas de la lista</label>
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
