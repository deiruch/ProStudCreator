﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public class ProjectSingleTask
    {
        public string project { get; set; }
        public string taskOrganiseExpert { get; set; }
        public string taskOrganiseRoom { get; set; }
        public string taskOrganiseDate { get; set; }
        public string taskPayExpert { get; set; }
    }

    public partial class AdminPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
            SelectedSemester.DataSource = db.Semester.OrderByDescending(s => s.StartDate);
            SelectedSemester.DataBind();
            SelectedSemester.SelectedValue = Semester.CurrentSemester(db).Id.ToString();
            SelectedSemester.Items.Insert(0, new ListItem("Alle Semester", ""));
            SelectedSemester.Items.Insert(1, new ListItem("――――――――――――――――", ".", false));
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShibUser.CanVisitAdminPage())
            {
                DivAdminProjects.Visible = ShibUser.CanPublishProject();
                DivExcelExport.Visible = ShibUser.CanExportExcel();


                if (!Page.IsPostBack)
                {
                    if (Session["SelectedAdminProjects"] == null)
                    {
                        radioSelectedProjects.SelectedIndex = 0;
                        Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
                    }
                    else
                    {
                        radioSelectedProjects.SelectedIndex = (int)Session["SelectedAdminProjects"];
                    }

                    if (Session["AdminProjectCollapsed"] == null)
                        CollapseAdminProjects(true);
                    else
                        CollapseAdminProjects((bool)Session["AdminProjectCollapsed"]);

                    if(Session["ExcelExportCollapsed"] == null)
                        CollapseExcelExport(true);
                    else
                        CollapseExcelExport((bool)Session["ExcelExportCollapsed"]);


                    if (Session["AddInfoCollapsed"] == null)
                        CollapseAddInfo(false);
                    else
                        CollapseAddInfo((bool)Session["AddInfoCollapsed"]);

                }

                CheckProjects.DataSource = GetSelectedProjects();
                CheckProjects.DataBind();
                //GVTasks.DataSource = AllTasks();
                //GVTasks.DataBind();
            }
            else
            {
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
            }

            Session["LastPage"] = "adminpage";
        }

        private ProjectSingleElement getProjectSingleElement(Project i)
        {
            return new ProjectSingleElement
            {
                id = i.Id,
                advisorName = string.Concat(new[]
                {
                    i.Advisor1Name != ""
                        ? "<a href=\"mailto:" + i.Advisor1Mail + "\">" +
                          Server.HtmlEncode(i.Advisor1Name).Replace(" ", "&nbsp;") + "</a>"
                        : "?",
                    i.Advisor2Name != ""
                        ? "<br /><a href=\"mailto:" + i.Advisor2Mail + "\">" +
                          Server.HtmlEncode(i.Advisor2Name).Replace(" ", "&nbsp;") + "</a>"
                        : ""
                }),
                projectName = (i.ProjectNr == 0 ? "" : i.ProjectNr.ToString("D2") + ": ") + i.Name,
                Institute = i.Department.DepartmentName,
                p5 = i.POneType.P5 || i.PTwoType != null && i.PTwoType.P5,
                p6 = i.POneType.P6 || i.PTwoType != null && i.PTwoType.P6,
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX
                                   ? "DesignUX"
                                   : (i.TypeHW
                                       ? "HW"
                                       : (i.TypeCGIP
                                           ? "CGIP"
                                           : (i.TypeMathAlg
                                               ? "MathAlg"
                                               : (i.TypeAppWeb
                                                   ? "AppWeb"
                                                   : (i.TypeDBBigData
                                                       ? "DBBigData"
                                                       : (i.TypeSysSec
                                                           ? "SysSec"
                                                           : (i.TypeSE ? "SE" : "Transparent")))))))) + ".png",
                projectType2 = "pictures/projectTyp" + (i.TypeHW && i.TypeDesignUX
                                   ? "HW"
                                   : (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)
                                       ? "CGIP"
                                       : (i.TypeMathAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)
                                           ? "MathAlg"
                                           : (i.TypeAppWeb &&
                                              (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg)
                                               ? "AppWeb"
                                               : (i.TypeDBBigData &&
                                                  (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg ||
                                                   i.TypeAppWeb)
                                                   ? "DBBigData"
                                                   : (i.TypeSysSec &&
                                                      (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMathAlg ||
                                                       i.TypeAppWeb || i.TypeDBBigData)
                                                       ? "SysSec"
                                                       : (i.TypeSE && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP ||
                                                                       i.TypeMathAlg || i.TypeAppWeb ||
                                                                       i.TypeDBBigData || i.TypeSysSec)
                                                           ? "SE"
                                                           : "Transparent"))))))) + ".png"
            };
        }

        private IQueryable<ProjectSingleElement> GetSelectedProjects()
        {
            var depId = ShibUser.GetDepartmentId(db);

            if (radioSelectedProjects.SelectedValue == "inProgress")
            {
                var lastSemStartDate = Semester.LastSemester(db).StartDate;
                return db.Projects.Where(p =>
                    p.DepartmentId == depId &&
                    p.ModificationDate > lastSemStartDate &&
                    (p.State == ProjectState.InProgress || p.State == ProjectState.Rejected || p.State == ProjectState.Submitted))
                    .OrderBy(i => i.Department.DepartmentName)
                    .ThenBy(i => i.ProjectNr)
                    .Select(i => getProjectSingleElement(i));
            }
            else
            {
                return db.Projects
                    .Where(item => item.State == ProjectState.Submitted && (int?)item.DepartmentId == depId)
                    .OrderBy(i => i.Department.DepartmentName)
                    .ThenBy(i => i.ProjectNr)
                    .Select(i => getProjectSingleElement(i));
            }
        }

        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
                return;

            var id = Convert.ToInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "revokeSubmission":
                    var projectr = db.Projects.Single(i => i.Id == id);
                    projectr.State = ProjectState.InProgress;
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "deleteProject":
                    var project = db.Projects.Single(i => i.Id == id);
                    project.Delete();
                    db.SubmitChanges();
                    Response.Redirect(Request.RawUrl);
                    break;
                case "editProject":
                    Response.Redirect("AddNewProject?id=" + id);
                    break;
                default:
                    throw new Exception("Unknown command " + e.CommandName);
            }
        }

        private void CollapseAdminProjects(bool collapse)
        {
            Session["AdminProjectCollapsed"] = collapse;
            DivAdminProjectsCollapsable.Visible = !collapse;
            btnAdminProjectsCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseExcelExport(bool collapse)
        {
            Session["ExcelExportCollapsed"] = collapse;
            DivExcelExportCollapsable.Visible = !collapse;
            btnExcelExportCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseAddInfo(bool collapse)
        {
            Session["AddInfoCollapsed"] = collapse;
            DivAddInfoCollapsable.Visible = !collapse;
            btnAddInfoCollapse.Text = collapse ? "◄" : "▼";
        }

        private ProjectSingleTask CheckDefenseOrganised(Project _project)
        {
            var task = new ProjectSingleTask();
            var empty = true;
            DateTime defensestart;
            if (!DateTime.TryParseExact(Semester.CurrentSemester(db).DefenseIP6Start, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out defensestart))
                throw new Exception(
                    "Die Daten in der Datenbank entsprechen nicht dem richtigen Format. Bitte melden Sie diesen Fehler einer Ansprechperson!");

            if (_project?.LogProjectTypeID == 2 && _project?.LogExpertID == null &&
                defensestart > DateTime.Now.AddMonths(-6) && _project.State == 3)
            {
                empty = false;
                task.taskOrganiseExpert = "pictures/experte.png";
            }
            else
            {
                task.taskOrganiseExpert = "pictures/projectTypTransparent.png";
            }

            if (_project?.LogProjectTypeID == 2 && _project?.LogDefenceDate == null &&
                defensestart > DateTime.Now.AddMonths(-6) && _project.State == 3)
            {
                empty = false;
                task.taskOrganiseDate = "pictures/datum.png";
            }
            else
            {
                task.taskOrganiseDate = "pictures/projectTypTransparent.png";
            }

            if (_project?.LogProjectTypeID == 2 && string.IsNullOrEmpty(_project.LogDefenceRoom) &&
                defensestart > DateTime.Now.AddMonths(-6) && _project.State == 3)
            {
                empty = false;
                task.taskOrganiseRoom = "pictures/raum.png";
            }
            else
            {
                task.taskOrganiseRoom = "pictures/projectTypTransparent.png";
            }

            if (_project?.LogProjectType?.P6 == true && _project?.LogExpertPaid != true &&
                _project?.LogDefenceDate > DateTime.Now && _project.State == 3)
            {
                empty = false;
                task.taskPayExpert = "pictures/experte_zahlen.png";
            }
            else
            {
                task.taskPayExpert = "pictures/projectTypTransparent.png";
            }
            task.project = _project?.Name;
            return empty ? null : task;
        }

        public IQueryable<ProjectSingleTask> AllTasks()
        {
            var projects = db.Projects.Select(i => i);

            var allTaskList = new List<ProjectSingleTask>();
            foreach (var project in projects)
                if (CheckDefenseOrganised(project) != null)
                    allTaskList.Add(CheckDefenseOrganised(project));
            return allTaskList.AsQueryable();
        }

        protected void btnMarketingExport_OnClick(object sender, EventArgs e)
        {
            IEnumerable<Project> projectsToExport = null;
            if (radioProjectStart.SelectedValue == "StartingProjects") //Projects which start in this Sem.
                if (SelectedSemester.SelectedValue == "") //Alle Semester
                {
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
                else
                {
                    var semesterId = int.Parse(SelectedSemester.SelectedValue);
                    projectsToExport = db.Projects
                        .Where(i => i.SemesterId == semesterId && i.State == ProjectState.Published &&
                                    i.LogStudent1Mail != null && i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
            else if (radioProjectStart.SelectedValue == "EndingProjects") //Projects which end in this Sem.
                if (SelectedSemester.SelectedValue == "") //Alle Semester
                {
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
                else
                {
                    var selectedSemester = db.Semester.Single(s => s.Id == int.Parse(SelectedSemester.SelectedValue));
                    var previousSemester = db.Semester.OrderByDescending(s => s.StartDate)
                        .FirstOrDefault(s => s.StartDate < selectedSemester.StartDate);
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != ""
                                    && (i.LogProjectDuration == 1 && i.Semester == selectedSemester ||
                                        i.LogProjectDuration == 2 && i.Semester == previousSemester))
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
            else
                throw new Exception($"Unexpected selection: {radioProjectStart.SelectedIndex}");

            //Response
            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition",
                $"attachment; filename={SelectedSemester.SelectedItem.Text}_IP56_Informatikprojekte.xlsx");
            ExcelCreator.GenerateMarketingList(Response.OutputStream, projectsToExport, db,
                SelectedSemester.SelectedItem.Text);
            Response.End();
        }

        protected void radioSelectedProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
            CheckProjects.DataSource = GetSelectedProjects();
            CheckProjects.DataBind();

        }
        protected void CheckProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var project = db.Projects.Single(item => item.Id == ((ProjectSingleElement)e.Row.DataItem).id);

            Color? col = null;
            switch (project.State)
            {
                case ProjectState.Published:
                    col = ColorTranslator.FromHtml("#A9F5A9");
                    break;
                case ProjectState.Rejected:
                    col = ColorTranslator.FromHtml("#F5A9A9");
                    break;
                case ProjectState.Submitted:
                    col = ColorTranslator.FromHtml("#ffcc99");
                    break;
            }
            if (!col.HasValue) return;
            foreach (TableCell cell in e.Row.Cells)
                cell.BackColor = col.Value;
        }

        protected void btnAdminProjectsCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAdminProjects(!(bool)Session["AdminProjectCollapsed"]);
        }

        protected void btnExcelExportCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseExcelExport(!(bool)Session["ExcelExportCollapsed"]);
        }

        protected void btnAddInfoCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAddInfo(!(bool)Session["AddInfoCollapsed"]);
        }
    }
}