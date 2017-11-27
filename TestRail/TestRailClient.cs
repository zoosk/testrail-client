using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using TestRail.Types;

namespace TestRail
{
    /// <summary>client used to access test case data in testrail</summary>
    public class TestRailClient
    {
        /// <summary>url for testrail</summary>
        protected string _URL_;
        /// <summary>testrail username</summary>
        protected string _UserName_;
        /// <summary>testrail password</summary>
        protected string _Password_;

        /// <summary>projects in the test rail database</summary>
        public List<Project> Projects => _Projects.Value;

        /// <summary>called when the client sends an http request</summary>
        public event EventHandler<HTTPRequestSentEventArgs> OnHTTPRequestSent = (s, e) => { };

        /// <summary>called when the client receives an http response</summary>
        public event EventHandler<string> OnHTTPResponseReceived = (s, e) => { };

        /// <summary>called when an operation fails</summary>
        public event EventHandler<string> OnOperationFailed = (s, e) => { };

        /// <summary>event args for http request sent</summary>
        public class HTTPRequestSentEventArgs : EventArgs
        {
            /// <summary>http method (GET, POST, PUT, DELETE, etc.)</summary>
            public string Method;
            /// <summary>uri</summary>
            public Uri Uri;
            /// <summary>post data</summary>
            public string PostContent;

            /// <summary>constructor</summary>
            /// <param name="method">http method used</param>
            /// <param name="uri">uri used</param>
            /// <param name="postContent">post content sent (if any)</param>
            public HTTPRequestSentEventArgs(string method, Uri uri, string postContent = null)
            {
                Method = method;
                Uri = uri;
                PostContent = postContent;
            }
        }

        // TODO: Add summary
        private readonly Lazy<List<Project>> _Projects;

        // TODO: Add summary
        private Dictionary<ulong, int> _PriorityIDToLevel => _LazyPriorityIDToLevel.Value;

        // TODO: Add summary
        private Lazy<Dictionary<ulong, int>> _LazyPriorityIDToLevel { get; set; }

        #region Constants
        protected const string _NODE_CASE_ = "case";
        protected const string _NODE_CASES_ = "cases";
        protected const string _NODE_CASE_TYPES_ = "case_types";
        protected const string _NODE_CASE_FIELDS_ = "case_fields";
        protected const string _NODE_MILESTONE_ = "milestone";
        protected const string _NODE_MILESTONES_ = "milestones";
        protected const string _NODE_PLAN_ = "plan";
        protected const string _NODE_PLANS_ = "plans";
        protected const string _NODE_PLAN_ENTRY_ = "plan_entry";
        protected const string _NODE_PROJECT_ = "project";
        protected const string _NODE_RESULTS_ = "results";
        protected const string _NODE_RESULTS_FOR_CASE_ = "results_for_case";
        protected const string _NODE_RESULTS_FOR_RUN_ = "results_for_run";
        protected const string _NODE_RUN_ = "run";
        protected const string _NODE_RUNS_ = "runs";
        protected const string _NODE_SECTION_ = "section";
        protected const string _NODE_SECTIONS_ = "sections";
        protected const string _NODE_SUITE_ = "suite";
        protected const string _NODE_SUITES_ = "suites";
        protected const string _NODE_TEST_ = "test";
        protected const string _NODE_TESTS_ = "tests";
        protected const string _NODE_USER_ = "user";
        protected const string _NODE_USERS_ = "users";
        #endregion Constants

        #region Constructor
        /// <summary>constructor</summary>
        /// <param name="url">url for test rail</param>
        /// <param name="username">user name</param>
        /// <param name="password">password</param>
        public TestRailClient(string url, string username, string password)
        {
            _URL_ = url;
            _UserName_ = username;
            _Password_ = password;

            _Projects = new Lazy<List<Project>>(GetProjects);

            // set up the lazy loading of the priority dictionary (priority id to priority value)
            _LazyPriorityIDToLevel = new Lazy<Dictionary<ulong, int>>(_CreatePrioritiesDict);
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Get the priority for the case if we can
        /// </summary>
        /// <param name="c">case to get the priority from</param>
        /// <returns>int value of priority if possible, null if not found</returns>
        public int? GetPriorityForCase(Case c)
        {
            int? priority = null;
            if (null != c?.PriorityID && null != _PriorityIDToLevel && _PriorityIDToLevel.ContainsKey(c.PriorityID.Value))
            {
                priority = _PriorityIDToLevel[c.PriorityID.Value];
            }
            return priority;
        }

        #region Add Commands
        /// <summary>adds a result for a test</summary>
        /// <param name="testID">id of the test</param>
        /// <param name="status">status of the result</param>
        /// <param name="comment">comment to log</param>
        /// <param name="version">version</param>
        /// <param name="elapsed">time elapsed to complete the test</param>
        /// <param name="defects">defects associated with the result</param>
        /// <param name="assignedToID">id of the user the result is assigned to</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddResult(ulong testID, ResultStatus? status, string comment = null, string version = null,
            TimeSpan? elapsed = null, string defects = null, ulong? assignedToID = null, JObject customs = null)
        {
            var uri = _CreateUri_(_CommandType_.add, "result", testID);
            var r = new Result { TestID = testID, StatusID = (ulong?)status, Comment = comment, Version = version, Elapsed = elapsed, Defects = defects, AssignedToID = assignedToID };
            var jsonParams = JsonUtility.Merge(r.GetJson(), customs);
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>creates a new test result for a test run and case combination</summary>
        /// <param name="runID">the id of the test run</param>
        /// <param name="CaseID">the id of the test case</param>
        /// <param name="status">status of the result</param>
        /// <param name="comment">comment to log</param>
        /// <param name="version">version</param>
        /// <param name="elapsed">time elapsed to complete the test</param>
        /// <param name="defects">defects associated with the result</param>
        /// <param name="assignedToID">id of the user the result is assigned to</param>
        /// <returns></returns>
        public CommandResult<ulong> AddResultForCase(ulong runID, ulong caseID, ResultStatus? status, string comment = null, string version = null,
            TimeSpan? elapsed = null, string defects = null, ulong? assignedToID = null, JObject customs = null)
        {
            var uri = _CreateUri_(_CommandType_.add, "result_for_case", runID, caseID);
            var r = new Result { StatusID = (ulong?)status, Comment = comment, Version = version, Elapsed = elapsed, Defects = defects, AssignedToID = assignedToID };
            var jsonParams = JsonUtility.Merge(r.GetJson(), customs);
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>adds a run</summary>
        /// <param name="projectID">id of the project</param>
        /// <param name="suiteID">id of the suite</param>
        /// <param name="name">name of the run</param>
        /// <param name="description">description of the run</param>
        /// <param name="milestoneID">id of the milestone</param>
        /// <param name="assignedToID">id of the user the run should be assigned to</param>
        ///<param name="caseIDs">(optional)an array of case IDs for the custom case selection, if null, then will include all case ids from the suite </param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddRun(ulong projectID, ulong suiteID, string name, string description, ulong milestoneID, ulong? assignedToID = null, HashSet<ulong> caseIDs = null)
        {
            var includeAll = true;

            // validates whether we are in include all or custom case selection mode
            if (null != caseIDs)
            {
                var atLeastOneCaseFoundInSuite = _CasesFoundInSuite(projectID, suiteID, caseIDs);
                if (atLeastOneCaseFoundInSuite)
                {
                    includeAll = false;
                }
                else
                {
                    return new CommandResult<ulong>(false, 0, new Exception("Case IDs not found in the Suite"));
                }
            }

            var uri = _CreateUri_(_CommandType_.add, _NODE_RUN_, projectID);
            var r = new Run { SuiteID = suiteID, Name = name, Description = description, MilestoneID = milestoneID, AssignedTo = assignedToID, IncludeAll = includeAll, CaseIDs = caseIDs };
            return _SendCommand(uri, r.GetJson());
        }

        /// <summary>Add a case</summary>
        /// <param name="sectionID">section id to add the case to</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeID">(optional)the ID of the case type</param>
        /// <param name="priorityID">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneID">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customFields">(optional)a json object for custom fields</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddCase(ulong sectionID, string title, ulong? typeID = null, ulong? priorityID = null, string estimate = null, ulong? milestoneID = null, string refs = null, JObject customFields = null)
        {
            return _AddCase_(sectionID, title, typeID, priorityID, estimate, milestoneID, refs, customFields);
        }

        /// <summary>Add a project</summary>
        /// <param name="projectName">the name of the project</param>
        /// <param name="announcement">(optional)the description of the project</param>
        /// <param name="showAnnouncement">(optional)true if the announcement should be displayed on the project's overview page and false otherwise</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddProject(string projectName, string announcement = null, bool? showAnnouncement = null)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(projectName)));
            }

            var uri = _CreateUri_(_CommandType_.add, _NODE_PROJECT_);
            var p = new Project { Name = projectName, Announcement = announcement, ShowAnnouncement = showAnnouncement };
            return _SendCommand(uri, p.GetJson());
        }

        /// <summary>creates a new section</summary>
        /// <param name="projectID">the ID of the project</param>
        /// <param name="suiteID">the ID of the test suite</param>
        /// <param name="name">the name of the section</param>
        /// <param name="parentID">(optional)the ID of the parent section (to build section hierarchies)</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddSection(ulong projectID, ulong suiteID, string name, ulong? parentID = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(_CommandType_.add, _NODE_SECTION_, projectID);
            var s = new Section { SuiteID = suiteID, ParentID = parentID, Name = name };
            return _SendCommand(uri, s.GetJson());
        }

        /// <summary>Creates a new test suite</summary>
        /// <param name="projectID">the ID of the project the test suite should be added to</param>
        /// <param name="name">the name of the test suite</param>
        /// <param name="description">(optional)the description of the test suite</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddSuite(ulong projectID, string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(_CommandType_.add, _NODE_SUITE_, projectID);
            var s = new Suite { Name = name, Description = description };
            return _SendCommand(uri, s.GetJson());
        }

        /// <summary>creates a new plan</summary>
        /// <param name="projectID">id of the project the test plan should be added to</param>
        /// <param name="name">name of the test plan</param>
        /// <param name="description">(optional)description of the test plan</param>
        /// <param name="milestoneID">(optional)id of the milestone to link the test plan</param>
        /// <param name="entries">an array of objects describing the test runs of the plan</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddPlan(ulong projectID, string name, string description = null, ulong? milestoneID = null, List<PlanEntry> entries = null)//todo:add config ids here
        // , params ulong[] suiteIDs)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(_CommandType_.add, _NODE_PLAN_, projectID);
            var p = new Plan { Name = name, Description = description, MilestoneID = milestoneID, Entries = entries };
            var jsonParams = p.GetJson();
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>Creates a new test run for a test plan</summary>
        /// <param name="planID">the ID of the plan the test run should be added to</param>
        /// <param name="suiteID">the ID of the test suite for the test run</param>
        /// <param name="name">(optional)the name of the test run</param>
        /// <param name="assignedToID">(optional)the ID of the user the test run should be assigned to</param>
        /// <param name="include_all">(optional)true for including all test cases of the test suite and false for a custom selection (default: true)</param>
        /// <returns></returns>
        public CommandResult<ulong> AddPlanEntry(ulong planID, ulong suiteID, string name = null, ulong? assignedToID = null, List<ulong> caseIDs = null)
        {
            var uri = _CreateUri_(_CommandType_.add, _NODE_PLAN_ENTRY_, planID);
            var pe = new PlanEntry { AssignedToID = assignedToID, SuiteID = suiteID, Name = name, CaseIDs = caseIDs };
            var jsonParams = pe.GetJson();
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>adds a milestone</summary>
        /// <param name="projectID">id of the project</param>
        /// <param name="name">name of the milestone</param>
        /// <param name="description">(optional)description of the milestone</param>
        /// <param name="dueOn">(optional)date on which the milestone is due</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddMilestone(ulong projectID, string name, string description = null, DateTime? dueOn = null)
        {
            var uri = _CreateUri_(_CommandType_.add, _NODE_MILESTONE_, projectID);
            var m = new Milestone { Name = name, Description = description, DueOn = dueOn };
            return _SendCommand(uri, m.GetJson());
        }
        #endregion Add Commands

        #region Update Commands
        /// <summary>update an existing case</summary>
        /// <param name="caseID">the ID of the test case</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeID">(optional)the ID of the case type</param>
        /// <param name="priorityID">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneID">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> UpdateCase(ulong caseID, string title, ulong? typeID = null, ulong? priorityID = null, string estimate = null, ulong? milestoneID = null, string refs = null)
        {
            return _UpdateCase_(caseID, title, typeID, priorityID, estimate, milestoneID, refs, null);
        }

        /// <summary>update an existing milestone</summary>
        /// <param name="milestoneID">id of the milestone</param>
        /// <param name="name">(optional)name of the milestone</param>
        /// <param name="description">(optional)description of the milestone</param>
        /// <param name="dueOn">(optional)date on which the milestone is due</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> UpdateMilestone(ulong milestoneID, string name = null, string description = null, DateTime? dueOn = null, bool? isCompleted = null)
        {
            var uri = _CreateUri_(_CommandType_.update, _NODE_MILESTONE_, milestoneID);
            var m = new Milestone { Name = name, Description = description, DueOn = dueOn, IsCompleted = isCompleted };
            return _SendCommand(uri, m.GetJson());
        }

        /// <summary>Update an existing plan</summary>
        /// <param name="planID">id of the plan</param>
        /// <param name="name">(optional)name of the test plan </param>
        /// <param name="description">(optional)the description of the test plan</param>
        /// <param name="milestoneID">(optional)the id of the milestone to link to the test plan</param>
        /// <returns></returns>
        public CommandResult<ulong> UpdatePlan(ulong planID, string name = null, string description = null, ulong? milestoneID = null)
        {
            var uri = _CreateUri_(_CommandType_.update, _NODE_PLAN_, planID);
            var p = new Plan { Name = name, Description = description, MilestoneID = milestoneID };
            var jsonParams = p.GetJson();
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>Creates a new test run for a test plan</summary>
        /// <param name="planID">the ID of the plan the test run should be added to</param>
        /// <param name="entryID">the ID of the test plan entry</param>
        /// <param name="name">(optional)the name of the test run</param>
        /// <param name="assignedToID">(optional)the ID of the user the test run should be assigned to</param>
        /// <param name="include_all">(optional)true for including all test cases of the test suite and false for a custom selection (default: true)</param>
        /// <returns></returns>
        public CommandResult<ulong> UpdatePlanEntry(ulong planID, string entryID, string name = null, ulong? assignedToID = null, List<ulong> caseIDs = null)
        {
            var uri = _CreateUri_(_CommandType_.update, _NODE_PLAN_ENTRY_, planID, null, null, entryID);
            var pe = new PlanEntry { AssignedToID = assignedToID, Name = name, CaseIDs = caseIDs };
            var jsonParams = pe.GetJson();
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>Update an existing project</summary>
        /// <param name="projectID">the id of the project</param>
        /// <param name="projectName">the name of the project</param>
        /// <param name="announcement">(optional)the description of the project</param>
        /// <param name="showAnnouncement">(optional)true if the announcement should be displayed on the project's overview page and false otherwise</param>
        /// <param name="isCompleted">(optional)specifies whether a project is considered completed or not</param>
        /// <returns></returns>
        public CommandResult<ulong> UpdateProject(ulong projectID, string projectName, string announcement = null, bool? showAnnouncement = null, bool? isCompleted = null)
        {
            var uri = _CreateUri_(_CommandType_.update, _NODE_PROJECT_, projectID);
            var p = new Project { Name = projectName, Announcement = announcement, ShowAnnouncement = showAnnouncement, IsCompleted = isCompleted };
            return _SendCommand(uri, p.GetJson());
        }

        /// <summary>update an existing test run</summary>
        /// <param name="runID">the id of an existing run</param>
        /// <param name="name">(optional)name of the test run</param>
        /// <param name="description">(optional)description of the test run</param>
        /// <param name="milestoneID">(optional)the id of the milestone to link to the test run</param>
        /// <param name="include_all">(optional)true for including all test cases of the test suite and false for a custom case selection</param>
        ///<param name="caseIDs">an array of case IDs for the custom case selection</param>
        /// <returns></returns>
        public CommandResult<ulong> UpdateRun(ulong runID, string name = null, string description = null, ulong? milestoneID = null, HashSet<ulong> caseIDs = null)
        {
            var includeAll = true;
            var run = GetRun(runID);

            // validates whether we are in include all or custom case selection mode
            if (null != run?.ProjectID && run.SuiteID.HasValue && null != caseIDs)
            {
                var atLeastOneCaseFoundInSuite = _CasesFoundInSuite(run.ProjectID.Value, run.SuiteID.Value, caseIDs);
                if (atLeastOneCaseFoundInSuite)
                {
                    includeAll = false;
                }
                else
                {
                    return new CommandResult<ulong>(false, 0, new Exception("Case IDs not found in the Suite"));
                }
            }

            var uri = _CreateUri_(_CommandType_.update, _NODE_RUN_, runID);
            var r = new Run { Name = name, Description = description, MilestoneID = milestoneID, IncludeAll = includeAll, CaseIDs = caseIDs };
            return _SendCommand(uri, r.GetJson());
        }

        /// <summary>Updates an existing section</summary>
        /// <param name="sectionID">id of the section to update</param>
        /// <param name="name">name of the section</param>
        /// <returns></returns>
        public CommandResult<ulong> UpdateSection(ulong sectionID, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(_CommandType_.update, _NODE_SECTION_, sectionID);
            var s = new Section { ID = sectionID, Name = name };
            return _SendCommand(uri, s.GetJson());
        }

        /// <summary>Update an existing suite</summary>
        /// <param name="suiteID">id of the suite to update</param>
        /// <param name="name">(optional)new name to update to</param>
        /// <param name="description">(optional)new description to update to</param>
        /// <returns></returns>
        public CommandResult<ulong> UpdateSuite(ulong suiteID, string name = null, string description = null)
        {
            var uri = _CreateUri_(_CommandType_.update, _NODE_SUITE_, suiteID);
            var s = new Suite { Name = name, Description = description };
            return _SendCommand(uri, s.GetJson());
        }
        #endregion Update Commands

        #region Close Commands
        /// <summary>closes a plan</summary>
        /// <param name="planID">id of the plan</param>
        /// <returns>true if successful</returns>
        public bool ClosePlan(ulong planID)
        {
            var uri = _CreateUri_(_CommandType_.close, _NODE_PLAN_, planID);

            var result = _CallPostEndpoint(uri);
            if (result.WasSuccessful)
            {
                var json = JObject.Parse(result.Value);
                return result.WasSuccessful;
            }

            OnOperationFailed(this, $"Could not close plan : {result.Value}");
            return false;
        }

        /// <summary>closes a run</summary>
        /// <param name="runID">id of the run</param>
        /// <returns>true if successful</returns>
        public bool CloseRun(ulong runID)
        {
            var uri = _CreateUri_(_CommandType_.close, _NODE_RUN_, runID);
            var result = _CallPostEndpoint(uri);
            if (result.WasSuccessful)
            {
                var json = JObject.Parse(result.Value);
                return result.WasSuccessful;
            }

            OnOperationFailed(this, $"Could not close run : {result.Value}");
            return false;
        }
        #endregion Close Commands

        #region Delete Commands
        /// <summary>Delete a milestone</summary>
        /// <param name="milestoneID">id of the milestone</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteMilestone(ulong milestoneID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_MILESTONE_, milestoneID);
            return _SendCommand(uri);
        }

        /// <summary>Delete a case</summary>
        /// <param name="caseID">id of the case to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteCase(ulong caseID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_CASE_, caseID);
            return _SendCommand(uri);
        }

        /// <summary>Delete a plan</summary>
        /// <param name="planID">id of the plan to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeletePlan(ulong planID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_PLAN_, planID);
            return _SendCommand(uri);
        }

        /// <summary>Delete a specific plan entry for a plan id</summary>
        /// <param name="planID">id of the plan</param>
        /// <param name="entryID">string representation of the GUID for the entryID</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeletePlanEntry(ulong planID, string entryID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_PLAN_ENTRY_, planID, null, null, entryID);
            return _SendCommand(uri);
        }

        /// <summary>Delete the Project</summary>
        /// <param name="projectID">id of the project to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteProject(ulong projectID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_PROJECT_, projectID);
            return _SendCommand(uri);
        }

        /// <summary>Delete the section</summary>
        /// <param name="sectionID">id of the section to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteSection(ulong sectionID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_SECTION_, sectionID);
            return _SendCommand(uri);
        }

        /// <summary>Delete the suite</summary>
        /// <param name="suiteID">id of the suite to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteSuite(ulong suiteID)
        {
            var uri = _CreateUri_(_CommandType_.delete, _NODE_SUITE_, suiteID);
            return _SendCommand(uri);
        }
        #endregion Delete Commands

        #region Get Commands
        /// <summary>gets a test</summary>
        /// <param name="testID">id of the test</param>
        /// <returns>information about the test</returns>
        public Test GetTest(ulong testID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_TEST_, testID);
            return _GetItem_(_NODE_TEST_, uri, Test.Parse);
        }

        /// <summary>gets tests associated with a run</summary>
        /// <param name="runID">id of the run</param>
        /// <returns>tests associated with the run</returns>
        public List<Test> GetTests(ulong runID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_TESTS_, runID);
            return _GetItems_(_NODE_TESTS_, uri, Test.Parse);
        }

        /// <summary>gets a case</summary>
        /// <param name="caseID">id of the case</param>
        /// <returns>information about the case</returns>
        public Case GetCase(ulong caseID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_CASE_, caseID);
            return _GetItem_(_NODE_CASE_, uri, Case.Parse);
        }

        /// <summary>gets cases associated with a suite</summary>
        /// <param name="proejctID">id of the project</param>
        /// <param name="suiteID">id of the suite</param>
        /// <param name="sectionID">(optional) id of the section</param>
        /// <returns>cases associated with the suite</returns>
        public List<Case> GetCases(ulong projectID, ulong suiteID, ulong? sectionID = null)
        {
            var optionalSectionID = sectionID.HasValue ? $"&section_id={sectionID.Value}" : string.Empty;
            var options = $"&suite_id={suiteID}{optionalSectionID}";
            var uri = _CreateUri_(_CommandType_.get, _NODE_CASES_, projectID, null, options);
            return _GetItems_(_NODE_CASES_, uri, Case.Parse);
        }

        // TODO: Add summary
        public List<CaseField> GetCaseFields()
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_CASE_FIELDS_);
            return _GetItems_(_NODE_CASE_TYPES_, uri, CaseField.Parse);
        }

        // TODO: Add summary
        public List<CaseType> GetCaseTypes()
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_CASE_TYPES_);
            return _GetItems_(_NODE_CASE_TYPES_, uri, CaseType.Parse);
        }

        /// <summary>gets a suite</summary>
        /// <param name="suiteID">id of the suite</param>
        /// <returns>information about the suite</returns>
        public Suite GetSuite(ulong suiteID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_SUITE_, suiteID);
            return _GetItem_(_NODE_SUITE_, uri, Suite.Parse);
        }

        /// <summary>gets suites associated with a project</summary>
        /// <param name="projectID">id of the project</param>
        /// <returns>suites associated with the project</returns>
        public List<Suite> GetSuites(ulong projectID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_SUITES_, projectID);
            return _GetItems_(_NODE_SUITES_, uri, Suite.Parse);
        }

        /// <summary>gets a section</summary>
        /// <param name="sectionID">id of the section</param>
        /// <returns>information about the section</returns>
        public Section GetSection(ulong sectionID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_SECTION_, sectionID);
            return _GetItem_(_NODE_SECTION_, uri, Section.Parse);
        }

        /// <summary>gets sections associated with a suite</summary>
        /// <param name="projectID">id of the project</param>
        /// <param name="suiteID">id of the suite</param>
        /// <returns>sections associated with the suite</returns>
        public List<Section> GetSections(ulong projectID, ulong suiteID)
        {
            var options = $"&suite_id={suiteID}";
            var uri = _CreateUri_(_CommandType_.get, _NODE_SECTIONS_, projectID, null, options);
            return _GetItems_(_NODE_SECTIONS_, uri, Section.Parse);
        }

        /// <summary>gets a run</summary>
        /// <param name="runID">id of the run</param>
        /// <returns>information about the run</returns>
        public Run GetRun(ulong runID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_RUN_, runID);
            return _GetItem_(_NODE_RUN_, uri, Run.Parse);
        }

        /// <summary>gets runs associated with a project</summary>
        /// <param name="projectID">id of the project</param>
        /// <returns>runs associated with the project</returns>
        public List<Run> GetRuns(ulong projectID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_RUNS_, projectID);
            return _GetItems_(_NODE_RUNS_, uri, Run.Parse);
        }

        /// <summary>gets a plan</summary>
        /// <param name="planID">id of the plan</param>
        /// <returns>information about the plan</returns>
        public Plan GetPlan(ulong planID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_PLAN_, planID);
            return _GetItem_(_NODE_PLAN_, uri, Plan.Parse);
        }

        /// <summary>gets plans associated with a project</summary>
        /// <param name="projectID">id of the project</param>
        /// <returns>plans associated with the project</returns>
        public List<Plan> GetPlans(ulong projectID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_PLANS_, projectID);
            return _GetItems_(_NODE_PLANS_, uri, Plan.Parse);
        }

        /// <summary>gets a milestone</summary>
        /// <param name="milestoneID">id of the milestone</param>
        /// <returns>information about the milestone</returns>
        public Milestone GetMilestone(ulong milestoneID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_MILESTONE_, milestoneID);
            return _GetItem_(_NODE_MILESTONE_, uri, Milestone.Parse);
        }

        /// <summary>gets milestones associated with a project</summary>
        /// <param name="projectID">id of the project</param>
        /// <returns>milestone associated with project</returns>
        public List<Milestone> GetMilestones(ulong projectID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_MILESTONES_, projectID);
            return _GetItems_(_NODE_MILESTONES_, uri, Milestone.Parse);
        }

        /// <summary>gets a project</summary>
        /// <param name="projectID">id of the project</param>
        /// <returns>information about the project</returns>
        public Project GetProject(ulong projectID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_PROJECT_, projectID);
            return _GetItem_(_NODE_PROJECT_, uri, Project.Parse);
        }

        /// <summary>gets all projects contained in the testrail instance</summary>
        /// <returns></returns>
        public List<Project> GetProjects()
        {
            const string nodeName = "projects";
            var uri = _CreateUri_(_CommandType_.get, nodeName);
            return _GetItems_(nodeName, uri, Project.Parse);
        }

        /// <summary>Get User for user id</summary>
        /// <param name="userID">user id to search for</param>
        /// <returns>a User object</returns>
        public User GetUser(ulong userID)
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_USER_, userID);
            return _GetItem_(_NODE_USER_, uri, User.Parse);
        }

        /// <summary>Find a user by their email address</summary>
        /// <param name="email">email address of the user</param>
        /// <returns>user if found</returns>
        public User GetUserByEmail(string email)
        {
            // validate the email string
            if (string.IsNullOrWhiteSpace(email))
            {
                return default(User);
            }

            const string nodeName = "user_by_email";
            var optional = $"&email={email}";
            var uri = _CreateUri_(_CommandType_.get, nodeName, null, null, optional);
            return _GetItem_(nodeName, uri, User.Parse);
        }

        /// <summary>Get a list of users in the testrail instance</summary>
        /// <returns>List of users</returns>
        public List<User> GetUsers()
        {
            var uri = _CreateUri_(_CommandType_.get, _NODE_USERS_);
            return _GetItems_(_NODE_USERS_, uri, User.Parse);
        }

        /// <summary>
        /// Returns a list of test results for a test 
        /// </summary>
        /// <param name="testID">id of the test</param>
        /// <param name="limit">(optional) maximum amount of test results to return, latest first</param>
        /// <returns></returns>
        public List<Result> GetResults(ulong testID, ulong? limit = null)
        {
            var optional = (limit.HasValue) ? $"&limit={limit.Value}" : string.Empty;
            var uri = _CreateUri_(_CommandType_.get, _NODE_RESULTS_, testID, null, optional);
            return _GetItems_(_NODE_RESULTS_, uri, Result.Parse);
        }

        /// <summary>
        /// Return the list of test results for a test run and the case combination
        /// </summary>
        /// <param name="runID">id of the test run</param>
        /// <param name="caseID">id of the test case</param>
        /// <param name="limit">(optional) maximum amount of test results to return, latest first</param>
        /// <returns>list of test results for a case</returns>
        public List<Result> GetResultsForCase(ulong runID, ulong caseID, ulong? limit = null)
        {
            var optional = limit.HasValue ? $"&limit={limit.Value}" : string.Empty;
            var uri = _CreateUri_(_CommandType_.get, _NODE_RESULTS_FOR_CASE_, runID, caseID, optional);
            return _GetItems_(_NODE_RESULTS_FOR_CASE_, uri, Result.Parse);
        }

        /// <summary>
        /// Return the list of test results for a test run
        /// </summary>
        /// <param name="runID">id of the rest run</param>
        /// <param name="limit">(optional) maximum amount of test results to return, latest first</param>
        /// <returns>list of test results for a test run</returns>
        public List<Result> GetResultsForRun(ulong runID, ulong? limit = null)
        {
            var optional = limit.HasValue ? $"&limit={limit.Value}" : string.Empty;
            var uri = _CreateUri_(_CommandType_.get, _NODE_RESULTS_FOR_RUN_, runID, null, optional);
            return _GetItems_(_NODE_RESULTS_FOR_RUN_, uri, Result.Parse);
        }

        /// <summary>
        /// Returns the list of statuses available to test rail
        /// </summary>
        /// <returns>list of possible statuses</returns>
        public List<Status> GetStatuses()
        {
            const string nodeName = "statuses";
            var uri = _CreateUri_(_CommandType_.get, nodeName);
            return _GetItems_(nodeName, uri, Status.Parse);
        }

        /// <summary>
        /// Get a list of all available priorities
        /// </summary>
        /// <returns>list of priorities</returns>
        public List<Priority> GetPriorities()
        {
            const string nodeName = "priorities";
            var uri = _CreateUri_(_CommandType_.get, nodeName);
            return _GetItems_(nodeName, uri, Priority.Parse);
        }

        /// <summary>
        /// Returns a list of Config Groups available in a Project
        /// </summary>
        /// <param name="projectID">ID of the Project to return the Config Groups for</param>
        /// <returns>list of ConfigurationGroup</returns>
        public List<ConfigurationGroup> GetConfigurationGroups(ulong projectID)
        {
            const string nodeName = "configs";
            var uri = _CreateUri_(_CommandType_.get, nodeName, projectID);
            return _GetItems_(nodeName, uri, ConfigurationGroup.Parse);
        }
        #endregion Get Commands
        #endregion Public Methods

        #region Protected Methods
        /// <summary>executes a get request for an item</summary>
        /// <typeparam name="T">the type of item</typeparam>
        /// <param name="nodeName">the name of item's node</param>
        /// <param name="parse">a method which parse json into the item</param>
        /// <param name="id">the id of the item</param>
        /// <returns>object of the supplied type containing information about the item</returns>
        protected T _GetItem_<T>(string nodeName, string uri, Func<JObject, T> parse)
            where T : BaseTestRailType, new()
        {
            var result = _CallTestRailGetEndpoint(uri);
            if (!result.WasSuccessful)
            {
                OnOperationFailed(this, $"Could not get {nodeName}: {result.Value}");
                return default(T);
            }

            var json = JObject.Parse(result.Value);
            return parse(json);
        }

        /// <summary>executes a get request for an item</summary>
        /// <typeparam name="T">the type of the item</typeparam>
        /// <param name="nodeName">the name of the item's node</param>
        /// <param name="parse">a method which parses the json into the item</param>
        /// <param name="id1">the id of the first item on which to filter the get request</param>
        /// <param name="id2">the id of the second item on which to filter the get request</param>
        /// <param name="options">additional options to append to the get request</param>
        /// <returns>list of objects of the supplied type corresponding th supplied filters</returns>
        protected List<T> _GetItems_<T>(string nodeName, string uri, Func<JObject, T> parse)
            where T : BaseTestRailType, new()
        {
            var items = new List<T>();
            var result = _CallTestRailGetEndpoint(uri);

            if (!result.WasSuccessful)
            {
                OnOperationFailed(this, $"Could not get {nodeName}s: {result.Value}");
            }
            else
            {
                var jarray = JArray.Parse(result.Value);
                if (null != jarray)
                {
                    items = JsonUtility.ConvertJArrayToList(jarray, parse);
                }
            }
            return items;
        }

        /// <summary>Creates a URI with the parameters given in the format</summary>
        /// <param name="uriType">the type of action the server is going to take (i.e. get, add, update, close)</param>
        /// <param name="nodeName"></param>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <param name="options"></param>
        /// <returns>the uri</returns>
        protected static string _CreateUri_(_CommandType_ uriType, string nodeName, ulong? id1 = null, ulong? id2 = null, string options = null, string id2Str = null)
        {
            var uri = $"?/api/v2/{uriType}_{nodeName}{(id1.HasValue ? "/" + id1.Value : string.Empty)}{(id2.HasValue ? "/" + id2.Value : !string.IsNullOrWhiteSpace(id2Str) ? "/" + id2Str : string.Empty)}{(!string.IsNullOrWhiteSpace(options) ? options : string.Empty)}";
            return uri;
        }

        /// <summary>Add a case</summary>
        /// <param name="sectionID">section id to add the case to</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeID">(optional)the ID of the case type</param>
        /// <param name="priorityID">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneID">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>result of the command</returns>
        protected CommandResult<ulong> _AddCase_(ulong sectionID, string title, ulong? typeID = null, ulong? priorityID = null, string estimate = null, ulong? milestoneID = null, string refs = null, JObject customs = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(title)));
            }

            var uri = _CreateUri_(_CommandType_.add, _NODE_CASE_, sectionID);
            var tmpCase = new Case { Title = title, TypeID = typeID, PriorityID = priorityID, Estimate = estimate, MilestoneID = milestoneID, References = refs };
            var jsonParams = JsonUtility.Merge(tmpCase.GetJson(), customs);
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>update an existing case</summary>
        /// <param name="caseID">the ID of the test case</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeID">(optional)the ID of the case type</param>
        /// <param name="priorityID">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneID">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customs">(optional)</param>
        /// <returns>result of the command</returns>
        protected CommandResult<ulong> _UpdateCase_(ulong caseID, string title, ulong? typeID = null, ulong? priorityID = null, string estimate = null, ulong? milestoneID = null, string refs = null, JObject customs = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(title)));
            }

            var uri = _CreateUri_(_CommandType_.update, _NODE_CASE_, caseID);
            var tmpCase = new Case { Title = title, TypeID = typeID, PriorityID = priorityID, Estimate = estimate, MilestoneID = milestoneID, References = refs };
            var jsonParams = JsonUtility.Merge(tmpCase.GetJson(), customs);
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>
        /// Command type's available
        /// </summary>
        protected enum _CommandType_
        {
            get,
            add,
            update,
            delete,
            close,
        }
        #endregion Protected Methods

        #region Private Methods
        /// <summary>makes an http get call to the testrail</summary>
        /// <param name="uri">uri of the endpoint</param>
        /// <returns>result of the call</returns>
        private CommandResult _CallTestRailGetEndpoint(string uri)
        {
            uri = _URL_ + uri;
            OnHTTPRequestSent(this, new HTTPRequestSentEventArgs("GET", new Uri(uri)));
            CommandResult cr;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.AllowAutoRedirect = true;
                var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{_UserName_}:{_Password_}"));
                request.Headers["Authorization"] = $"Basic {authInfo}";
                request.UserAgent = "TestRail Client for .NET";
                request.Method = "GET";
                request.Accept = "application/json";
                request.ContentType = "application/json";

                // receive the response
                var response = (HttpWebResponse)request.GetResponse();
                var responseDataStream = response.GetResponseStream();
                var reader = new StreamReader(responseDataStream);
                var responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                cr = new CommandResult(response.StatusCode == HttpStatusCode.OK, responseFromServer);
            }
            catch (Exception e) { cr = new CommandResult(false, e.ToString()); }
            if (!cr.WasSuccessful)
            {
                OnOperationFailed(this, $"HTTP RESPONSE: {cr.Value}");
            }
            else
            {
                OnHTTPResponseReceived(this, cr.Value);
            }
            return cr;
        }

        /// <summary>makes an http post call to the testrail</summary>
        /// <param name="uri">uri of the endpoint</param>
        /// <param name="postParams">post parameters alternating between keys and values</param>
        /// <returns>result of the call</returns>
        private CommandResult _CallPostEndpoint(string uri, JObject json = null)
        {
            uri = _URL_ + uri;
            string postContent = null;
            if (null != json)
            {
                postContent = json.ToString();
            }
            OnHTTPRequestSent(this, new HTTPRequestSentEventArgs("POST", new Uri(uri), postContent));

            CommandResult cr;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.AllowAutoRedirect = true;
                var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{_UserName_}:{_Password_}"));
                request.Headers["Authorization"] = $"Basic {authInfo}";
                request.UserAgent = "TestRail Client for .NET";
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = 0;
                request.Accept = "application/json";

                // add post data to the request
                if (!string.IsNullOrWhiteSpace(postContent))
                {
                    var byteArray = Encoding.UTF8.GetBytes(postContent);
                    request.ContentLength = byteArray.Length;
                    var requestDataStream = request.GetRequestStream();
                    requestDataStream.Write(byteArray, 0, byteArray.Length);
                    requestDataStream.Close();
                }

                // receive the response
                var response = (HttpWebResponse)request.GetResponse();
                var responseDataStream = response.GetResponseStream();
                var reader = new StreamReader(responseDataStream);
                var responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                cr = new CommandResult(response.StatusCode == HttpStatusCode.OK, responseFromServer);
            }
            catch (Exception e) { cr = new CommandResult(false, e.ToString()); }
            if (!cr.WasSuccessful)
            {
                OnOperationFailed(this, $"HTTP RESPONSE: {cr.Value}");
            }
            else
            {
                OnHTTPResponseReceived(this, cr.Value);
            }
            return cr;
        }

        /// <summary>Send a command to the server</summary>
        /// <param name="uri">uri to send</param>
        /// <param name="jsonParams">parameter</param>
        /// <returns></returns>
        private CommandResult<ulong> _SendCommand(string uri, JObject jsonParams = null)
        {
            Exception exe = null;
            ulong resultValue = 0;
            var wasSuccessful = false;

            try
            {
                var result = _CallPostEndpoint(uri, jsonParams);
                wasSuccessful = result.WasSuccessful;
                if (wasSuccessful)
                {
                    if (!string.IsNullOrWhiteSpace(result.Value))
                    {
                        var json = JObject.Parse(result.Value);
                        var token = json["id"];

                        try
                        {
                            if (null == token)
                            {
                                // do nothing
                            }
                            else if (JTokenType.String == token.Type)
                            {
                                // for plan entry 
                                resultValue = (ulong)(json["runs"][0]["id"]);
                            }
                            else if (JTokenType.Integer == token.Type)
                            {
                                resultValue = (ulong)json["id"];
                            }
                        }
                        catch
                        {
                            // do nothing since result value is already 0 
                        }
                    }
                }
                else
                {
                    exe = new Exception(result.Value);
                }
            }
            catch (Exception e)
            {
                exe = e;
            }
            return new CommandResult<ulong>(wasSuccessful, resultValue, exe);
        }

        /// <summary>
        /// Determines if at least one of the case ids given is contained in the project and suite 
        /// </summary>
        /// <param name="projectID">id of the project</param>
        /// <param name="suiteID">id of the suite</param>
        /// <param name="caseIDs"></param>
        /// <returns></returns>
        private bool _CasesFoundInSuite(ulong projectID, ulong suiteID, ICollection<ulong> caseIDs)
        {
            var validCases = GetCases(projectID, suiteID);
            return validCases.Any(tmpCase => tmpCase.ID.HasValue && caseIDs.Contains(tmpCase.ID.Value));
        }

        /// <summary>
        /// Create a priority dictionary 
        /// </summary>
        /// <returns>dictionary of priority ID (from test rail) to priority levels(where Higher value means higher priority)</returns>
        private Dictionary<ulong, int> _CreatePrioritiesDict()
        {
            var tmpDict = new Dictionary<ulong, int>();
            var priorityList = GetPriorities();
            foreach (var priority in priorityList.Where(priority => null != priority))
            {
                tmpDict[priority.ID] = priority.PriorityLevel;
            }
            return tmpDict;
        }
        #endregion Private Methods
    }
}
