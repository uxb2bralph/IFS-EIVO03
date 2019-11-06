<%@ Control Language="C#" AutoEventWireup="true"  %>
<%@ Register Src="~/Module/Common/DataModelCache.ascx" TagPrefix="uc1" TagName="DataModelCache" %>

<input type="hidden" name="pageToken" value="<%= modelItem.DataItem %>" />
<script>
    $(function () {
        $('a').filter(function (index, element) {
            return $(this).prop('onclick') == null;
        }).on('click', function (evt) {
            var event = event || window.event;
            event.preventDefault();
            var $this = $(this);
            if ($this.prop('href').indexOf('?') > 0) {
                window.location.href = $this.prop('href') + '&pageToken=' + $('input[name="pageToken"]').val();
            }
            else {
                window.location.href = $this.prop('href') + '?pageToken=' + $('input[name="pageToken"]').val();
            }
            //$('form').prop('action', $this.prop('href')).submit();
        });
    });
</script>
<uc1:DataModelCache runat="server" ID="modelItem" KeyName="tokenID" />
<script runat="server">

    bool _required;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.PreRender += module_common_antiforgery_ascx_PreRender;
    }

    void module_common_antiforgery_ascx_PreRender(object sender, EventArgs e)
    {
        if (_required)
        {
            if (modelItem.DataItem !=null 
                && !(Request["pageToken"] == (String)modelItem.DataItem 
                    || Request.Form["pageToken"] == (String)modelItem.DataItem))
            {
                throw new Exception("Invalid Request!!");
            }
            modelItem.DataItem = Guid.NewGuid().ToString();
        }
    }

    [System.ComponentModel.Bindable(true)]
    public bool Required
    {
        get
        {
            return _required;
        }
        set
        {
            _required = value;
            if (!_required)
            {
                modelItem.DataItem = null;
            }
        }
    }
   

</script>
