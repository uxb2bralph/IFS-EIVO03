<%@ Control Language="C#" AutoEventWireup="true" Inherits="eIVOGo.Module.JsGrid.DataField.JsGridField" %>
<%@ Import Namespace="eIVOGo.Resource.Module.JsGrid.DataField" %>
<% if (!String.IsNullOrEmpty(FieldVariable)) { %>
<script>
<%= FieldVariable%>[<%= FieldVariable%>.length] = {
    "name": "InvoiceNo",
    "type": "text",
    "title": "<%=InvoiceNo.發票號碼%>",
    "width": "120",
    "align": "center",
    itemTemplate: function (value, item) {
        return $('<a>')
            .attr('href', '#')
            .html(value)
            .on('click', function (evt) {
                showInvoiceModal(item.DocID);
            });
    }
};
</script>
<% } %>