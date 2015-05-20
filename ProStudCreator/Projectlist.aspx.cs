﻿using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace ProStudCreator
{
    public class ProjectSingleElement
    {
        public int id { get; set; }
        public string Institute { get; set; }
        public string advisorName { get; set; }
        public string projectName { get; set; }
        public string projectType1 { get; set; }
        public string projectType2 { get; set; }
        public bool p5 { get; set; }
        public bool p6 { get; set; }
    }

    public partial class Projectlist : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected PlaceHolder AdminView;
        protected GridView CheckProjects;
        protected RadioButtonList ListFilter;
        protected GridView AllProjects;
        protected Button newProject;
        protected PlaceHolder AdminViewPDF;
        protected Button AllProjectsAsPDF;
        protected void Page_Load(object sender, EventArgs e)
        {
            IQueryable<Project> projects =
                from i in db.Projects
                where true
                select i;
            if (ShibUser.IsAdmin())
            {
                AllProjectsAsPDF.Visible = true;
                AdminView.Visible = true;
                AdminViewPDF.Visible = true;
                CheckProjects.DataSource =
                    from item in projects
                    where (int)item.State == 1 && (int?)item.DepartmentId == ShibUser.GetDepartmentId()
                    select item into i
                    select getProjectSingleElement(i);
                if (ListFilter.SelectedValue == "AllFutureProjects")
                {
                    projects =
                        from p in projects
                        where p.PublishedDate >= Semester.CurrentSemester.StartDate && (int)p.State == 3
                        select p;
                }
                else if (ListFilter.SelectedValue == "AllPastProjects")
                {
                    projects =
                        from p in projects
                        where p.PublishedDate >= (Semester.CurrentSemester - 1).StartDate && p.PublishedDate <= (Semester.CurrentSemester - 1).EndDate && (int)p.State == 3
                        select p;
                }
                else if (ListFilter.SelectedValue == "InProgress")
                {
                    projects =
                        from item in projects
                        where item.Creator == ShibUser.GetEmail() && (int)item.State == 0
                        select item;
                }
                else if (ListFilter.SelectedValue == "Submitted")
                {
                    projects =
                        from item in projects
                        where item.Creator == ShibUser.GetEmail() && (int)item.State == 1
                        select item;
                }
                else
                {
                    projects =
                        from item in projects
                        where item.Creator == ShibUser.GetEmail() && (int)item.State == 3
                        select item;
                }
            }
            else
            {
                ListFilter.Items[0].Attributes.CssStyle.Add("display", "none");
                ListFilter.Items[1].Attributes.CssStyle.Add("display", "none");
                if (ListFilter.SelectedValue == "InProgress")
                {
                    projects =
                        from item in projects
                        where item.Creator == ShibUser.GetEmail() && (int)item.State == 0
                        select item;
                }
                else if (ListFilter.SelectedValue == "Submitted")
                {
                    projects =
                        from item in projects
                        where item.Creator == ShibUser.GetEmail() && (int)item.State == 1
                        select item;
                }
                else
                {
                    projects =
                        from item in projects
                        where item.Creator == ShibUser.GetEmail() && (int)item.State == 3
                        select item;
                }
            }
            AllProjects.DataSource =
                from i in projects
                select getProjectSingleElement(i);
            CheckProjects.DataBind();
            AllProjects.DataBind();
        }
        private ProjectSingleElement getProjectSingleElement(Project i)
        {
            return new ProjectSingleElement
            {
                id = i.Id,
                advisorName = string.Concat(new string[]
                {
                    "<a href=\"mailto:",
                    i.Advisor1Mail,
                    "\">",
                    base.Server.HtmlEncode(i.Advisor1Name).Replace(" ", "&nbsp;"),
                    "</a><br /><a href=\"mailto:",
                    i.Advisor2Mail,
                    "\">",
                    base.Server.HtmlEncode(i.Advisor2Name).Replace(" ", "&nbsp;"),
                    "</a>"
                }),
                projectName = ((i.ProjectNr == 0) ? "??" : i.ProjectNr.ToString()) + ": " + i.Name,
                Institute = i.Department.DepartmentName,
                p5 = i.POneType.P5 || (i.PTwoType != null && i.PTwoType.P5),
                p6 = i.POneType.P6 || (i.PTwoType != null && i.PTwoType.P6),
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX ? "DesignUX" : (i.TypeHW ? "HW" : (i.TypeCGIP ? "CGIP" : (i.TypeMathAlg ? "MathAlg" : (i.TypeAppWeb ? "AppWeb" : (i.TypeDBBigData ? "DBBigData" : "Transparent")))))) + ".png",
                projectType2 = "pictures/projectTyp" + ((i.TypeHW && i.TypeDesignUX) ? "HW" : ((i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)) ? "CGIP" : ((i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)) ? "MathAlg" : ((i.TypeAppWeb && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)) ? "AppWeb" : ((i.TypeDBBigData && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg || i.TypeAppWeb)) ? "DBBigData" : "Transparent"))))) + ".png"
            };
        }
        protected void AllProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Project project = db.Projects.Single((Project item) => item.Id == ((ProjectSingleElement)e.Row.DataItem).id);
                Color? col = null;
                if (project.State == ProjectState.Published)
                {
                    col = new Color?(ColorTranslator.FromHtml("#A9F5A9"));
                    if (!ShibUser.IsAdmin())
                    {
                        e.Row.Cells[e.Row.Cells.Count - 1].Visible = false;
                    }
                }
                else if (project.State == ProjectState.Rejected)
                {
                    col = new Color?(ColorTranslator.FromHtml("#F5A9A9"));
                }
                else if (project.OverOnePage)
                {
                    col = new Color?(ColorTranslator.FromHtml("#F3F781"));
                }
                if (col.HasValue)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        cell.BackColor = col.Value;
                    }
                }
            }
        }
        protected void newProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddNewProject");
        }
        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            string commandName = e.CommandName;
            if (commandName != null)
            {
                if (!(commandName == "showProject"))
                {
                    if (!(commandName == "editProject"))
                    {
                        if (!(commandName == "deleteProject"))
                        {
                            if (!(commandName == "revokeSubmission"))
                            {
                                if (commandName == "SinglePDF")
                                {
                                    int idPDF = System.Convert.ToInt32(e.CommandArgument);
                                    CreateSinglePDF(idPDF);
                                }
                            }
                            else
                            {
                                Project projectr = db.Projects.Single((Project i) => i.Id == System.Convert.ToInt32(e.CommandArgument));
                                projectr.State = ProjectState.InProgress;
                                db.SubmitChanges();
                                Response.Redirect(Request.RawUrl);
                            }
                        }
                        else
                        {
                            Project project = db.Projects.Single((Project i) => i.Id == System.Convert.ToInt32(e.CommandArgument));
                            project.Delete();
                            db.SubmitChanges();
                            Response.Redirect(Request.RawUrl);
                        }
                    }
                    else
                    {
                        Response.Redirect("AddNewProject?id=" + e.CommandArgument);
                    }
                }
                else
                {
                    Response.Redirect("AddNewProject?id=" + e.CommandArgument + "&show=true");
                }
            }
        }
        private void CreateSinglePDF(int idPDF)
        {
            float margin = Utilities.MillimetersToPoints(System.Convert.ToSingle(20));
            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, margin, margin, margin, margin))
                {
                    PdfCreator pdfCreator = new PdfCreator();
                    pdfCreator.AppendToPDF(document, output, Enumerable.Repeat<int>(idPDF, 1));
                }
                bytesInStream = output.ToArray();
            }
            var project = db.Projects.Single(i => i.Id == idPDF);
            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=" + project.Department.DepartmentName + project.ProjectNr.ToString("00") + ".pdf");
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }
        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            if (AllProjects.Rows.Count != 0)
            {
                float margin = Utilities.MillimetersToPoints(System.Convert.ToSingle(20));
                byte[] bytesInStream;
                using (var output = new MemoryStream())
                {
                    using (var document = new Document(PageSize.A4, margin, margin, margin, margin))
                    {
                        PdfCreator pdfCreator = new PdfCreator();
                        pdfCreator.AppendToPDF(document, output,
                            from pse in (IEnumerable<ProjectSingleElement>)AllProjects.DataSource
                            select pse.id);
                    }
                    bytesInStream = output.ToArray();
                }
                Response.Clear();
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", "attachment; filename=AllProjects.pdf");
                Response.BinaryWrite(bytesInStream);
                Response.End();
            }
            else
            {
                string message = "In dieser Kategorie sind keine Projekte vorhanden!";
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(base.GetType(), "alert", sb.ToString());
            }
        }
    }
}
