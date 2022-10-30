using Portal.Constant;
using Portal.Resources;
using Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    public static class HtmlExtensions
    {
        #region Get Current Url
        public static string GetCurrentUrl(string CurrentArea, string CurrentController)
        {
            string CurrentUrl = "";
            if (!string.IsNullOrEmpty(CurrentArea))
            {
                CurrentUrl = string.Format("{0}/{1}", CurrentArea, CurrentController);
            }
            else
            {
                CurrentUrl = CurrentController;
            }
            return CurrentUrl;
        }
        #endregion Get Current Url
        #region Save Button
        public static MvcHtmlString SaveButton(string id, string btn_name , object htmlAttributes = null)
        {
            TagBuilder aTag = new TagBuilder("a");
            aTag.Attributes.Add("id", id);

            aTag.Attributes.Add("class", "btn btn-white btn-inverse btn-sm noBorder");
            aTag.Attributes.Add("onclick", "$(this).button('loading')");
            aTag.InnerHtml += string.Format("<i class='fa fa-save bigger-120 blue'></i>");
            aTag.InnerHtml += string.Format("<span class='spinner-border spinner-border-sm d-none' role='status' aria-hidden='true'></span> {0}", btn_name);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                aTag.MergeAttributes(attributes, true);
            }
            aTag.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(aTag.ToString());
        }

        public static MvcHtmlString SaveEditButtonPermission(string id, string btn_name, string areaName, string controlName, object htmlAttributes = null)
        {
            
            string CurrentUrl = GetCurrentUrl(areaName, controlName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Edit);
            if(isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("id", id);
              
                aTag.Attributes.Add("class", "btn bg-blue");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.InnerHtml += string.Format("<i class='fa fa-save'></i>");
                aTag.InnerHtml += string.Format("<span class='spinner-border spinner-border-sm d-none' role='status' aria-hidden='true'></span> {0}", btn_name);
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
            
        }

        #endregion
        #region Search Button
        public static MvcHtmlString SearchButton(object htmlAttributes = null)
        {
            TagBuilder aTag = new TagBuilder("a");
            aTag.Attributes.Add("id", "btn-search");
            aTag.Attributes.Add("class", "btn btn-primary btn-search");
            aTag.InnerHtml += string.Format("<i class='fa fa-search'></i> {0}","");
            aTag.InnerHtml += string.Format("<span class='spinner-border spinner-border-sm d-none' role='status' aria-hidden='true'></span> {0}", LanguageResource.Btn_Search);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                aTag.MergeAttributes(attributes, true);
            }
            aTag.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(aTag.ToString());
        }
        #endregion
        #region Actived Icon
        public static MvcHtmlString ActivedIcon(bool? actived, object htmlAttributes = null)
        {
            TagBuilder iIcon = new TagBuilder("i");
            if (actived == true)
            {
                iIcon.Attributes.Add("class", "fa fa-check true-icon");
            }
            else
            {
                iIcon.Attributes.Add("class", "fa fa-close false-icon");
            }
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                iIcon.MergeAttributes(attributes, true);
            }
            iIcon.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(iIcon.ToString());
        }
        #endregion Actived Icon

        #region Create Button
        public static MvcHtmlString CreateButton(string areaName, string controlName, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controlName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Create);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("id", "btn-create");
                aTag.Attributes.Add("class", "btn bg-blue");
                aTag.InnerHtml += string.Format("<i class='fa fa-plus-square'></i> {0}", LanguageResource.Btn_Create);
                aTag.Attributes.Add("href", string.Format("/{0}/Create", CurrentUrl));
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
            
        }
        #endregion Create Button
        #region Back Button
        public static MvcHtmlString BackButton(string areaName, string controllerName, object htmlAttributes = null)
        {
            //a Tag
            TagBuilder aTag = new TagBuilder("a");
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            aTag.Attributes.Add("href", string.Format("/{0}/", CurrentUrl));
            aTag.InnerHtml += LanguageResource.Btn_Back;

            //small Tag
            TagBuilder smallTag = new TagBuilder("small");
            smallTag.InnerHtml += string.Format("<i class='fa fa-arrow-circle-left'></i>");
            smallTag.InnerHtml += aTag.ToString();
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                smallTag.MergeAttributes(attributes, true);
            }
            smallTag.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(smallTag.ToString());
        }
        #endregion Back Button
        #region Required Textbox
        public static MvcHtmlString RequiredTextboxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            var result = new StringBuilder();

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlAttributes == null || (htmlAttributes != null && htmlAttributes.GetType().GetProperty("class") == null))
            {
                attributes.Add("class", "form-control");
            }
            result.Append(helper.TextBoxFor(expression, attributes));
            result.Append(helper.ValidationMessageFor(expression, null));
            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<div class=\"input-group-btn\"><span class=\"required\">*</span></div>");
            //}
            //result.AppendFormat("{0}</div>",
            //    helper.ValidationMessageFor(expression, null, new { @class = "validation-text" }));

            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RequiredTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var result = new StringBuilder();

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlAttributes == null || (htmlAttributes != null && htmlAttributes.GetType().GetProperty("class") == null))
            {
                attributes.Add("class", "form-control");
            }
            result.Append(helper.TextAreaFor(expression, attributes));
            result.Append(helper.ValidationMessageFor(expression, null));
            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<div class=\"input-group-btn\"><span class=\"required\">*</span></div>");
            //}
            //result.AppendFormat("{0}</div>",
            //    helper.ValidationMessageFor(expression, null, new { @class = "validation-text" }));

            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString RequiredTextboxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool? isUpperText, object htmlAttributes)
        {
            var result = new StringBuilder();

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attributes.Add("class", "form-control");

            if (isUpperText == true)
            {
                attributes.Add("onkeyup", "$(this).val($(this).val().toUpperCase())");
            }

            //result.AppendFormat("<div class=\"input-group input-group-required\">{0}",
            //    helper.TextBoxFor(expression, attributes));
            result.Append(helper.TextBoxFor(expression, attributes));

            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<div class=\"input-group-btn\"><span class=\"required\">*</span></div>");
            //}
            //result.AppendFormat("{0}</div>",
            //    helper.ValidationMessageFor(expression, null));
            result.Append(helper.ValidationMessageFor(expression, null));

            return MvcHtmlString.Create(result.ToString());
        }

        //Format text: datetime, decimal,...
        public static MvcHtmlString RequiredTextboxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string format, object htmlAttributes = null)
        {
            var result = new StringBuilder();

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);


            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            attributes.Add("class", "form-control");

            //result.AppendFormat("<div class=\"input-group input-group-required\">{0}", helper.TextBoxFor(expression, format, attributes));
            result.Append(helper.TextBoxFor(expression, format, attributes));
            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<div class=\"input-group-btn\"><span class=\"required\">*</span></div>");
            //}
            //result.AppendFormat("{0}</div>",
            //    helper.ValidationMessageFor(expression, null, new { @class = "validation-text" }));

            result.Append(helper.ValidationMessageFor(expression, null));

            return MvcHtmlString.Create(result.ToString());
        }


        #endregion Required Textbox

        #region DateEditorFor

        public static MvcHtmlString RequiredDateboxFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var result = new StringBuilder();

            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            //var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //if (htmlAttributes == null || (htmlAttributes != null && htmlAttributes.GetType().GetProperty("class") == null))
            //{
            //    attributes.Add("class", "form-control");
            //}
            result.Append(helper.EditorFor(expression, htmlAttributes));
            result.Append(helper.ValidationMessageFor(expression, null));
            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<div class=\"input-group-btn\"><span class=\"required\">*</span></div>");
            //}
            //result.AppendFormat("{0}</div>",
            //    helper.ValidationMessageFor(expression, null, new { @class = "validation-text" }));

            return MvcHtmlString.Create(result.ToString());
        }
        # endregion DateEditorFor
        #region Radio Button
        public static MvcHtmlString ActivedRadioButton<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //tag div
            result.Append("<div class='div-radio'>");
            //2 radio button
            //result.AppendFormat("<label class=\"label-radio\">{0} {1}</label><label class=\"label-radio\">{2} {3}</label>",
            //    helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
            //    LanguageResource.Actived_True,
            //    helper.RadioButtonFor(expression, false),
            //    LanguageResource.Actived_False);
            //end tag div

            result.Append(helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }));

            result.Append("&nbsp" + LanguageResource.Actived_True);
            result.Append("&nbsp" + helper.RadioButtonFor(expression, false));
            result.Append("&nbsp" + LanguageResource.Actived_False);
            result.Append("</div>");
            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString ActivedRadioButtonGender<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //tag div
            result.Append("<div class='div-radio'>");
            //2 radio button
            //result.AppendFormat("<label class=\"label-radio\">{0} {1}</label><label class=\"label-radio\">{2} {3}</label>",
            //    helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
            //    LanguageResource.Actived_True,
            //    helper.RadioButtonFor(expression, false),
            //    LanguageResource.Actived_False);
            //end tag div

            result.Append(helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }));

            result.Append("&nbsp" + LanguageResource.Male);
            result.Append("&nbsp" + helper.RadioButtonFor(expression, false));
            result.Append("&nbsp" + LanguageResource.Female);
            result.Append("</div>");
            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString ActivedRadioButtonIs<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //tag div
            result.Append("<div class='div-radio'>");
            //2 radio button
            result.AppendFormat("<label class=\"label-radio\">{0} {1}</label><label class=\"label-radio\">{2} {3}</label>",
                helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
                LanguageResource.Yes,
                helper.RadioButtonFor(expression, false),
                LanguageResource.No);
            //end tag div
            result.Append("</div>");

            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString MaterialActivedRadioButton<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //tag div
            result.Append("<div class='div-radio'>");
            //2 radio button
            result.AppendFormat("<label class=\"label-radio\">{0} {1}</label><label class=\"label-radio\">{2} {3}</label>",
                helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
                LanguageResource.Material_Actived_True,
                helper.RadioButtonFor(expression, false),
                LanguageResource.Material_Actived_False);
            //end tag div
            result.Append("</div>");

            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString HelmetAdultRadioButton<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //tag div
            result.Append("<div class='div-radio'>");
            //2 radio button
            result.AppendFormat("<label class=\"label-radio\">{0} {1}</label><label class=\"label-radio\">{2} {3}</label>",
                helper.RadioButtonFor(expression, true, new { @checked = "checked", @id = "" }),
                LanguageResource.Accessory_isHelmetAdult,
                helper.RadioButtonFor(expression, false),
                LanguageResource.Accessory_isHelmetChildren);
            //end tag div
            result.Append("</div>");

            return MvcHtmlString.Create(result.ToString());
        }
        #endregion
        #region TooltipLabelFor
        public static MvcHtmlString TooltipLabelForRequired<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //Begin Tag1
            //  result.Append("<div class=\"label-wrapper\">");
            //Add label text
            result.Append(helper.LabelFor(expression, new { @class = "control-label" }));
            //Get resource name to set up the tooltip
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            //if (!string.IsNullOrEmpty(metadata.Description))
            //{
            //    result.AppendFormat("<div class=\"ico-help\" title=\"{0}\"><i class=\"fa fa-question-circle\"></i></div>", metadata.Description);
            //}
            if (metadata.IsRequired)
            {
                result.Append(
                    "<span class=\"required\">&nbsp*</span>");
            }
            //End Tag1
            //   result.Append("</div>");
            return MvcHtmlString.Create(result.ToString());
        }
        public static MvcHtmlString TooltipLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new StringBuilder();
            //Begin Tag1
            //  result.Append("<div class=\"label-wrapper\">");
            //Add label text
            result.Append(helper.LabelFor(expression, new { @class = "control-label" }));
            //Get resource name to set up the tooltip
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            //if (!string.IsNullOrEmpty(metadata.Description))
            //{
            //    result.AppendFormat("<div class=\"ico-help\" title=\"{0}\"><i class=\"fa fa-question-circle\"></i></div>", metadata.Description);
            //}
            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<span class=\"required\">*</span>");
            //}
            //End Tag1
            //   result.Append("</div>");
            return MvcHtmlString.Create(result.ToString());
        }
        #endregion TooltipLabelFor

        #region View button
        public static MvcHtmlString ViewButton(string areaName, string controllerName, Guid id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.View);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("class", "btn btn-xs btn-default btn-view");

                aTag.Attributes.Add("href", string.Format("/{0}/View/{1}", CurrentUrl, id));
                aTag.Attributes.Add("onclick", "$(this).button('loading')");

                aTag.InnerHtml += string.Format("<i class='fa fa-eye'></i> {0}", "");
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        #endregion View button

        #region Edit Button in List
        public static MvcHtmlString EditButton(string areaName, string controllerName, Guid id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Edit);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("class", "btn btn-xs btn-default bg-gradient-info btn-edit");

                aTag.Attributes.Add("href", string.Format("/{0}/Edit/{1}", CurrentUrl, id));
                aTag.Attributes.Add("onclick", "$(this).button('loading')");

                aTag.InnerHtml += string.Format("<i class='fa fa-pen'></i> {0}", "");
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }

        //FunctionModel: FunctionId type string
        public static MvcHtmlString EditButton(string areaName, string controllerName, string id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Edit);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("class", "btn btn-xs btn-default btn-edit");
                aTag.Attributes.Add("href", string.Format("/{0}/Edit/{1}", CurrentUrl, id));
                aTag.Attributes.Add("onclick", "$(this).button('loading')");

                aTag.InnerHtml += string.Format("<i class='fa fa-pen'></i> {0}", LanguageResource.Btn_Edit);
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }

        //CategoryModel (ACTest)
        public static MvcHtmlString EditButton(string areaName, string controllerName, string id, string compid, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Edit);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("class", "btn btn-xs btn-default btn-edit");
                aTag.Attributes.Add("href", string.Format("/{0}/Edit/{1}?compid={2}", CurrentUrl, id, compid));
                aTag.Attributes.Add("onclick", "$(this).button('loading')");

                aTag.InnerHtml += string.Format("<i class='fa fa-pen'></i> {0}", LanguageResource.Btn_Edit);
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        #endregion Edit Button in List

        #region Delete Button
        public static MvcHtmlString DeleteButton(string areaName, string controllerName, string itemName, Guid id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Delete);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.Attributes.Add("class", "btn btn-xs btn-danger btn-delete");

                aTag.Attributes.Add("data-id", string.Format("{0}", id));
                aTag.Attributes.Add("data-current-url", string.Format("{0}", CurrentUrl));
                aTag.Attributes.Add("data-item-name", string.Format("{0}", itemName));
                aTag.InnerHtml += string.Format("<i class=\"fa fa-trash\"></i> {0}", "");
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        //FunctionModel: FunctionId type string
        public static MvcHtmlString DeleteButton(string areaName, string controllerName, string itemName, string id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Delete);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.Attributes.Add("class", "btn btn-xs btn-danger btn-delete");
                aTag.Attributes.Add("data-id", string.Format("{0}", id));
                aTag.Attributes.Add("data-current-url", string.Format("{0}", CurrentUrl));
                aTag.Attributes.Add("data-item-name", string.Format("{0}", itemName.ToLower()));
                aTag.InnerHtml += string.Format("<i class=\"fa fa-trash\"></i> {0}", LanguageResource.Btn_Delete);
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        //NC_Material3DFileModel
        public static MvcHtmlString DeleteButton(string areaName, string controllerName, string itemName, int id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Delete);
            //bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.Attributes.Add("class", "btn btn-danger btn-delete");

                aTag.Attributes.Add("data-id", string.Format("{0}", id));
                aTag.Attributes.Add("data-current-url", string.Format("{0}", CurrentUrl));
                aTag.Attributes.Add("data-item-name", string.Format("{0}", itemName));
                aTag.InnerHtml += string.Format("<i class=\"fa fa-trash-o\"></i> {0}", LanguageResource.Btn_Delete);
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        #endregion Delete Button
        #region Button send email
        public static MvcHtmlString SendMailButton(string areaName, string controllerName, string itemName, Guid id, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controllerName);
           // bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Delete);
            bool isHasPermission = true;
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.Attributes.Add("class", "btn bg-gradient-info btn-sendemail");

                aTag.Attributes.Add("data-id", string.Format("{0}", id));
                aTag.Attributes.Add("data-current-url", string.Format("{0}", CurrentUrl));
                aTag.Attributes.Add("data-item-name", string.Format("{0}", itemName));
                aTag.InnerHtml += string.Format("<i class=\"fa fa-envelope\"></i> {0}", itemName);
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        #endregion Button send email

        #region Export Button
        public static MvcHtmlString ExportButton(string areaName, string controlName, object htmlAttributes = null)
        {
            //button
            TagBuilder button = new TagBuilder("button");
            //button.Attributes.Add("id", "btn-export");
            button.Attributes.Add("class", "btn btn-success");
            button.InnerHtml += string.Format(LanguageResource.Btn_Export);
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                button.MergeAttributes(attributes, true);
            }
            button.ToString(TagRenderMode.SelfClosing);

            //button dropdown
            TagBuilder buttonDropdown = new TagBuilder("button");
            buttonDropdown.Attributes.Add("class", "btn btn-success dropdown-toggle");
            buttonDropdown.Attributes.Add("data-toggle", "dropdown");
            buttonDropdown.InnerHtml += "<span class='caret'></span><span class='sr-only'>&nbsp;</span>";
            buttonDropdown.ToString(TagRenderMode.SelfClosing);

            string CurrentUrl = GetCurrentUrl(areaName, controlName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Export);
            if (!isHasPermission)
            {
                //button.Attributes.Add("disabled", "disabled");
                //return new MvcHtmlString(button.ToString());

                return null;
            }


            //ul li
            TagBuilder ulTag = new TagBuilder("ul");
            ulTag.Attributes.Add("class", "dropdown-menu");
            ulTag.Attributes.Add("role", "menu");

            //li: Export Create
            TagBuilder liCreate = new TagBuilder("li");
            TagBuilder aTagExportCreate = new TagBuilder("a");
            aTagExportCreate.Attributes.Add("class", "btn-export");
            aTagExportCreate.Attributes.Add("href", string.Format("/{0}/ExportCreate", CurrentUrl));
            aTagExportCreate.InnerHtml += string.Format("<i class='fa fa-file-excel'></i> {0}", LanguageResource.Template_Add);
            liCreate.InnerHtml = aTagExportCreate.ToString();
            liCreate.ToString(TagRenderMode.SelfClosing);

            //li: Export Edit
            TagBuilder liEdit = new TagBuilder("li");
            TagBuilder aTagExportEdit = new TagBuilder("a");
            aTagExportEdit.Attributes.Add("class", "btn-export");
            aTagExportEdit.Attributes.Add("href", string.Format("/{0}/ExportEdit", CurrentUrl));
            aTagExportEdit.InnerHtml += string.Format("<i class='fa fa-file-excel'></i> {0}", LanguageResource.Template_Update);
            liEdit.InnerHtml = aTagExportEdit.ToString();
            liEdit.ToString(TagRenderMode.SelfClosing);

            ulTag.InnerHtml = liCreate.ToString() + liEdit.ToString();

            //div
            var div = new TagBuilder("div");
            div.AddCssClass("btn-group");
            div.InnerHtml = button.ToString() + buttonDropdown.ToString() + ulTag.ToString();
            div.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(div.ToString());
        }
        #endregion Export Button
        #region Import Button
        public static MvcHtmlString ImportButton(string areaName, string controlName, object htmlAttributes = null)
        {
            TagBuilder button = new TagBuilder("button");
            button.Attributes.Add("id", "btn-import");
            button.Attributes.Add("class", "btn bg-olive");
            button.Attributes.Add("data-toggle", "modal");
            button.Attributes.Add("data-target", "#importexcel-window");
            button.InnerHtml += string.Format("<i class='fa fa-upload'></i> {0}", LanguageResource.Btn_Import);
            string CurrentUrl = GetCurrentUrl(areaName, controlName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Import);
            if (!isHasPermission)
            {
                //button.Attributes.Add("disabled", "disabled");
                //return new MvcHtmlString(button.ToString());
                return null;
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                button.MergeAttributes(attributes, true);
            }
            button.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(button.ToString());
        }
        #endregion Import Button
        #region Get Permission
        public static bool GetPermission(string PageUrl, string Function)
        {
            //Get PageUrl from user input
            string pageUrl = string.Format("/{0}", PageUrl);
            //Get PageUrl from Session["Menu"]
            PermissionViewModel permission = (PermissionViewModel)HttpContext.Current.Session["Menu"];
            var pageId = permission.PageModel.Where(p => p.PageUrl == pageUrl)
                                            .Select(p => p.PageId)
                                            .FirstOrDefault();
            //Compare with PageId in PagePermission
            var pagePermission = permission.PagePermissionModel.Where(p => p.PageId == pageId && p.FunctionId == Function).FirstOrDefault();
            if (pagePermission != null)
            {
                return true;
            }
            return false;
        }
        #endregion Get Permission
        #region Required Dropdownlist For
        public static MvcHtmlString RequiredDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes = null)
        {
            return RequiredDropDownListBuilder(helper, expression, selectList, optionLabel, htmlAttributes: htmlAttributes);
        }

        private static MvcHtmlString RequiredDropDownListBuilder<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes = null)
        {
            var result = new StringBuilder();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (htmlAttributes == null || (htmlAttributes != null && htmlAttributes.GetType().GetProperty("class") == null))
            {
                attributes.Add("class", "form-control");
                attributes.Add("data-val", "true");
                attributes.Add("data-val-required", string.Format(LanguageResource.Required, metadata.DisplayName));
            }

            //result.AppendFormat("<div class=\"input-group input-group-required\">{0}",
            //   helper.DropDownListFor(expression, selectList, optionLabel, attributes));
            result.Append(helper.DropDownListFor(expression, selectList, optionLabel, attributes));

            //if (metadata.IsRequired)
            //{
            //    result.Append(
            //        "<div class=\"input-group-btn\"><span class=\"required\">*</span></div>");
            //}
            //result.AppendFormat("{0}</div>",
            //    helper.ValidationMessageFor(expression, null, new { @class = "validation-text" }));

            result.Append(helper.ValidationMessageFor(expression, null, new { @class = "validation-text" }));

            return MvcHtmlString.Create(result.ToString());
        }
        #endregion

        #region checkbox
        public static MvcHtmlString CheckbokInline(string name,string id)
        {
            var result = new StringBuilder();
            result.Append("<div class=\"icheck-primary d-inline\" style=\"margin-top:30px\">");
            result.AppendFormat("<input type=\"checkbox\" id=\"{0}\">",id);
            result.AppendFormat("<label for=\"{0}\">{1}",id,name);
            result.Append("</label>");
            result.Append("</div>");
            return MvcHtmlString.Create(result.ToString());
        }
        #endregion checkbox

        #region Button Approval cancel
        public static MvcHtmlString ApprovalButton( string areaName, string controlName, Guid? id = null, object htmlAttributes = null)
        {
            string CurrentUrl = GetCurrentUrl(areaName, controlName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Approval);
            if (isHasPermission)
            {
                TagBuilder aTag = new TagBuilder("a");
                aTag.Attributes.Add("data-url", CurrentUrl);
                if (id != null)
                {
                    aTag.Attributes.Add("data-id", id.ToString());
                }
                aTag.Attributes.Add("id", "btn-appval");
                aTag.Attributes.Add("class", "btn btn-info");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.InnerHtml += string.Format("<i class='fa fa-check'></i>");
                aTag.InnerHtml += string.Format("<span class='spinner-border spinner-border-sm d-none' role='status' aria-hidden='true'></span> {0}", "Duyệt");
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }

        public static MvcHtmlString CancelButton( string areaName, string controlName,Guid? id = null, object htmlAttributes = null)
        {
            TagBuilder aTag = new TagBuilder("a");
            string CurrentUrl = GetCurrentUrl(areaName, controlName);
            bool isHasPermission = GetPermission(CurrentUrl, ConstFunction.Cancel);
            if (isHasPermission)
            {
                aTag.Attributes.Add("data-url", CurrentUrl);
                if (id != null)
                {
                    aTag.Attributes.Add("data-id", id.ToString());
                }
                aTag.Attributes.Add("id", "btn-cancel");
                aTag.Attributes.Add("class", "btn btn-danger");
                aTag.Attributes.Add("onclick", "$(this).button('loading')");
                aTag.InnerHtml += string.Format("<i class='fa fa-ban'></i>");
                aTag.InnerHtml += string.Format("<span class='spinner-border spinner-border-sm d-none' role='status' aria-hidden='true'></span> {0}", "Hủy");
                if (htmlAttributes != null)
                {
                    var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                    aTag.MergeAttributes(attributes, true);
                }
                aTag.ToString(TagRenderMode.SelfClosing);
                return new MvcHtmlString(aTag.ToString());
            }
            return null;
        }
        #endregion  Button Approval cancel

        #region Button Workday
        public static MvcHtmlString ButtonWorkday()
        {

            //button dropdown
            TagBuilder buttonDropdown = new TagBuilder("a");
            //buttonDropdown.Attributes.Add("class", "info-box-icon bg-info");
            buttonDropdown.Attributes.Add("data-toggle", "dropdown");
            buttonDropdown.ToString(TagRenderMode.SelfClosing);

            // div dropdown
            TagBuilder divdropdownMenu = new TagBuilder("div");
            divdropdownMenu.AddCssClass("dropdown-menu");
            divdropdownMenu.Attributes.Add("role", "menu");

            string CurrentUrl = "#";


            //li: Export Create
            TagBuilder aTagCreate = new TagBuilder("a");
            aTagCreate.Attributes.Add("class", "dropdown-item");
            //aTagCreate.Attributes.Add("href", string.Format("/{0}/ExportCreate", CurrentUrl));
            aTagCreate.Attributes.Add("href", "#");
            aTagCreate.InnerHtml += string.Format("<i class='fa fa-plus-square'></i>{0}", LanguageResource.Create);

            //li: Export Edit
            TagBuilder aTagEdit = new TagBuilder("a");
            aTagEdit.Attributes.Add("class", "dropdown-item");
            //aTagEdit.Attributes.Add("href", string.Format("/{0}/ExportEdit", CurrentUrl));
            aTagEdit.Attributes.Add("href", "#");
            aTagEdit.InnerHtml += string.Format("<i class='fa fa-pen'></i>{0}", LanguageResource.Edit);

            //li: Export delete
            TagBuilder aTagDel = new TagBuilder("a");
            aTagDel.Attributes.Add("class", "dropdown-item");
            //aTagDel.Attributes.Add("href", string.Format("/{0}/ExportEdit", CurrentUrl));
            aTagDel.Attributes.Add("href", "#");
            aTagDel.InnerHtml += string.Format("<i class='fa fa-trash'></i>{0}", LanguageResource.Btn_Delete);

            divdropdownMenu.InnerHtml = aTagCreate.ToString() + aTagEdit.ToString() + aTagDel.ToString();

            //div
            buttonDropdown.InnerHtml = "<i class='fs-20 fa fa-cog'></i>" + divdropdownMenu.ToString();
            var div = new TagBuilder("div");
            div.AddCssClass("btn-group");
            div.InnerHtml = buttonDropdown.ToString();
            div.ToString(TagRenderMode.SelfClosing);
            return new MvcHtmlString(div.ToString());

        }

        #endregion Button Workday
    }
}
