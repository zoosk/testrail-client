using EnumStringValues;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TestRail.Enums;
using TestRail.Types;
using TestRail.Utils;

namespace TestRail
{
    /// <summary>client used to access test case data in testrail</summary>
    public class TestRailClient
    {
        /// <summary>url for testrail</summary>
        protected string Url;
        /// <summary>base 64 string of the given credentials</summary>
        protected string AuthInfo;

        /// <summary>projects in the test rail database</summary>
        public List<Project> Projects => _projects.Value;

        /// <summary>called when the client sends an http request</summary>
        public event EventHandler<HttpRequestSentEventArgs> OnHttpRequestSent = (s, e) => { };

        /// <summary>called when the client receives an http response</summary>
        public event EventHandler<string> OnHttpResponseReceived = (s, e) => { };

        /// <summary>called when an operation fails</summary>
        public event EventHandler<string> OnOperationFailed = (s, e) => { };

        /// <inheritdoc />
        /// <summary>event args for http request sent</summary>
        public class HttpRequestSentEventArgs : EventArgs
        {
            /// <summary>http method (GET, POST, PUT, DELETE, etc.)</summary>
            public string Method;

            /// <summary>uri</summary>
            public Uri Uri;

            /// <summary>post data</summary>
            public string PostContent;

            /// <inheritdoc />
            /// <summary>constructor</summary>
            /// <param name="method">http method used</param>
            /// <param name="uri">uri used</param>
            /// <param name="postContent">post content sent (if any)</param>
            public HttpRequestSentEventArgs(string method, Uri uri, string postContent = null)
            {
                Method = method;
                Uri = uri;
                PostContent = postContent;
            }
        }

        /// <summary>list of projects in the current testrail instance</summary>
        private readonly Lazy<List<Project>> _projects;

        /// <summary>dictionary of priority ID (from test rail) to priority levels(where Higher value means higher priority)</summary>
        private Dictionary<ulong, int> PriorityIdToLevel => LazyPriorityIdToLevel.Value;

        /// <summary>dictionary of priority ID (from test rail) to priority levels(where Higher value means higher priority)</summary>
        private Lazy<Dictionary<ulong, int>> LazyPriorityIdToLevel { get; }

        #region Constructor
        /// <summary>constructor</summary>
        /// <param name="url">url for test rail</param>
        /// <param name="username">user name</param>
        /// <param name="password">password</param>
        public TestRailClient(string url, string username, string password)
        {
            Url = url;
            AuthInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{username}:{password}"));

            _projects = new Lazy<List<Project>>(GetProjects);

            // set up the lazy loading of the priority dictionary (priority id to priority value)
            LazyPriorityIdToLevel = new Lazy<Dictionary<ulong, int>>(_CreatePrioritiesDict);
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>Get the priority for the case if we can</summary>
        /// <param name="c">case to get the priority from</param>
        /// <returns>int value of priority if possible, null if not found</returns>
        public int? GetPriorityForCase(Case c)
        {
            int? priority = null;

            if (null != c?.PriorityId && null != PriorityIdToLevel && PriorityIdToLevel.ContainsKey(c.PriorityId.Value))
            {
                priority = PriorityIdToLevel[c.PriorityId.Value];
            }

            return priority;
        }

        #region Add Commands
        /// <summary>
        /// Adds a new test result, comment or assigns a test.
        /// It's recommended to use AddResults() instead if you plan to add results for multiple tests.
        /// </summary>
        /// <param name="testId">The ID of the test the result should be added to.</param>
        /// <param name="status">The test status.</param>
        /// <param name="comment">The comment/description for the test result.</param>
        /// <param name="version">The version or build you tested against.</param>
        /// <param name="elapsed">The time it took to execute the test, e.g. "30s" or "1m 45s".</param>
        /// <param name="defects">A comma-separated list of defects to link to the test result.</param>
        /// <param name="assignedToId">The ID of a user the test should be assigned to.</param>
        /// <param name="customs">Custom fields are supported as well and must be submitted with their system name, prefixed with 'custom_', e.g. custom_comment</param>
        /// <returns>If successful, this method will return the new test result.</returns>
        public RequestResult<Result> AddResult(ulong testId, ResultStatus? status, string comment = null,
            string version = null, TimeSpan? elapsed = null, string defects = null, ulong? assignedToId = null, JObject customs = null)
        {
            var uri = _CreateUri_(CommandType.Add, CommandAction.Result, testId);

            var result = new Result
            {
                TestId = testId,
                StatusId = (ulong?)status,
                Comment = comment,
                Version = version,
                Elapsed = elapsed,
                Defects = defects,
                AssignedToId = assignedToId
            };

            var jsonParams = JsonUtility.Merge(result.GetJson(), customs);

            return SendCommand<Result>(uri, jsonParams);
        }

        /// <summary>
        /// Adds a new test result, comment or assigns a test.
        /// It's recommended to use AddResultsForCases() instead if you plan to add results for multiple test cases.
        /// </summary>
        /// <param name="runId">The ID of the test run.</param>
        /// <param name="caseId">The ID of the test case.</param>
        /// <param name="status">The test status.</param>
        /// <param name="comment">The comment/description for the test result.</param>
        /// <param name="version">The version or build you tested against.</param>
        /// <param name="elapsed">The time it took to execute the test, e.g. "30s" or "1m 45s".</param>
        /// <param name="defects">A comma-separated list of defects to link to the test result.</param>
        /// <param name="assignedToId">The ID of a user the test should be assigned to.</param>
        /// <param name="customs">Custom fields are supported as well and must be submitted with their system name, prefixed with 'custom_', e.g. custom_comment</param>
        /// <returns>If successful, this method will return the new test result.</returns>
        public RequestResult<Result> AddResultForCase(ulong runId, ulong caseId, ResultStatus? status, string comment = null,
            string version = null, TimeSpan? elapsed = null, string defects = null, ulong? assignedToId = null, JObject customs = null)
        {
            var uri = _CreateUri_(CommandType.Add, CommandAction.ResultForCase, runId, caseId);

            var result = new Result
            {
                StatusId = (ulong?)status,
                Comment = comment,
                Version = version,
                Elapsed = elapsed,
                Defects = defects,
                AssignedToId = assignedToId
            };

            var jsonParams = JsonUtility.Merge(result.GetJson(), customs);

            return SendCommand<Result>(uri, jsonParams);
        }

        // TODO: - Add a method called AddResultsForCases()
        // http://docs.gurock.com/testrail-api2/reference-results#add_results_for_cases

        /// <summary>Creates a new test run.</summary>
        /// <param name="projectId">The ID of the project the test run should be added to.</param>
        /// <param name="suiteId">The ID of the test suite for the test run (optional if the project is operating in single suite mode, required otherwise).</param>
        /// <param name="name">	The name of the test run.</param>
        /// <param name="description">The description of the test run.</param>
        /// <param name="milestoneId">The ID of the milestone to link to the test run.</param>
        /// <param name="assignedToId">The ID of the user the test run should be assigned to.</param>
        /// <param name="caseIds">An array of case IDs for the custom case selection.</param>
        /// <param name="customs">Custom fields are supported as well and must be submitted with their system name, prefixed with 'custom_', e.g. custom_comment</param>
        /// <returns>If successful, this method returns the new test run.</returns>
        public RequestResult<Run> AddRun(ulong projectId, ulong suiteId, string name, string description, ulong milestoneId,
            ulong? assignedToId = null, HashSet<ulong> caseIds = null, JObject customs = null)
        {
            var includeAll = true;

            // validates whether we are in include all or custom case selection mode
            if (null != caseIds)
            {
                var atLeastOneCaseFoundInSuite = _CasesFoundInSuite(projectId, suiteId, caseIds);

                if (atLeastOneCaseFoundInSuite)
                {
                    includeAll = false;
                }

                else
                {
                    return new RequestResult<Run>(HttpStatusCode.BadRequest, thrownException: new Exception("Case ids not found in the Suite"));
                }
            }

            var uri = _CreateUri_(CommandType.Add, CommandAction.Run, projectId);

            var run = new Run
            {
                SuiteId = suiteId,
                Name = name,
                Description = description,
                MilestoneId = milestoneId,
                AssignedTo = assignedToId,
                IncludeAll = includeAll,
                CaseIds = caseIds
            };

            var jsonParams = JsonUtility.Merge(run.GetJson(), customs);

            return SendCommand<Run>(uri, jsonParams);
        }

        /// <summary>Add a case</summary>
        /// <param name="sectionId">section id to add the case to</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeId">(optional)the ID of the case type</param>
        /// <param name="priorityId">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneId">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customFields">(optional)a json object for custom fields</param>
        /// <returns>result of the command</returns>
        public CommandResult<ulong> AddCase(ulong sectionId, string title, ulong? typeId = null, ulong? priorityId = null,
            string estimate = null, ulong? milestoneId = null, string refs = null, JObject customFields = null)
        {
            // TODO: Update to RequestResult
            return _AddCase_(sectionId, title, typeId, priorityId, estimate, milestoneId, refs, customFields);
        }

        /// <summary>Creates a new project (admin status required).</summary>
        /// <param name="projectName">The name of the project (required).</param>
        /// <param name="announcement">The description of the project.</param>
        /// <param name="showAnnouncement">True if the announcement should be displayed on the project's overview page and false otherwise.</param>
        /// <returns>If successful, this method returns the new project.</returns>
        public RequestResult<Project> AddProject(string projectName, string announcement = null, bool? showAnnouncement = null)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return new RequestResult<Project>(HttpStatusCode.BadRequest, thrownException: new ArgumentNullException(nameof(projectName)));
            }

            var uri = _CreateUri_(CommandType.Add, CommandAction.Project);

            var project = new Project
            {
                Name = projectName,
                Announcement = announcement,
                ShowAnnouncement = showAnnouncement
            };

            return SendCommand<Project>(uri, project.GetJson());
        }

        /// <summary>Creates a new section.</summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="suiteId">The ID of the test suite (ignored if the project is operating in single suite mode, required otherwise).</param>
        /// <param name="name">The name of the section (required).</param>
        /// <param name="parentId">The ID of the parent section (to build section hierarchies).</param>
        /// <param name="description">The description of the section (added with TestRail 4.0).</param>
        /// <returns>If successful, this method returns the new section.</returns>
        public RequestResult<Section> AddSection(ulong projectId, ulong suiteId, string name, ulong? parentId = null, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new RequestResult<Section>(HttpStatusCode.BadRequest, thrownException: new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(CommandType.Add, CommandAction.Section, projectId);

            var section = new Section
            {
                SuiteId = suiteId,
                ParentId = parentId,
                Name = name,
                Description = description
            };

            return SendCommand<Section>(uri, section.GetJson());
        }

        /// <summary>Creates a new test suite.</summary>
        /// <param name="projectId">The ID of the project the test suite should be added to.</param>
        /// <param name="name">The name of the test suite (required).</param>
        /// <param name="description">The description of the test suite.</param>
        /// <returns>If successful, this method returns the new test suite.</returns>
        public RequestResult<Suite> AddSuite(ulong projectId, string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new RequestResult<Suite>(HttpStatusCode.BadRequest, thrownException: new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(CommandType.Add, CommandAction.Suite, projectId);

            var suite = new Suite
            {
                Name = name,
                Description = description
            };

            return SendCommand<Suite>(uri, suite.GetJson());
        }

        /// <summary>Creates a new test plan.</summary>
        /// <param name="projectId">The ID of the project the test plan should be added to.</param>
        /// <param name="name">The name of the test plan (required).</param>
        /// <param name="description">The description of the test plan.</param>
        /// <param name="milestoneId">The ID of the milestone to link to the test plan.</param>
        /// <param name="entries">An array of objects describing the test runs of the plan.</param>
        /// <param name="customs">Custom fields are supported as well and must be submitted with their system name, prefixed with 'custom_', e.g. custom_comment</param>
        /// <returns>If successful, this method returns the new test plan.</returns>
        public RequestResult<Plan> AddPlan(ulong projectId, string name, string description = null, ulong? milestoneId = null,
            List<PlanEntry> entries = null, JObject customs = null)// TODO: - Add config ids here: , params ulong[] suiteIDs)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new RequestResult<Plan>(HttpStatusCode.BadRequest, thrownException: new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(CommandType.Add, CommandAction.Plan, projectId);

            var plan = new Plan
            {
                Name = name,
                Description = description,
                MilestoneId = milestoneId,
                Entries = entries
            };

            var jsonParams = JsonUtility.Merge(plan.GetJson(), customs);

            return SendCommand<Plan>(uri, jsonParams);
        }

        /// <summary>Adds one or more new test runs to a test plan.</summary>
        /// <param name="planId">The ID of the plan the test runs should be added to.</param>
        /// <param name="suiteId">The ID of the test suite for the test run(s) (required).</param>
        /// <param name="name">The name of the test run(s).</param>
        /// <param name="assignedToId">The ID of the user the test run(s) should be assigned to.</param>
        /// <param name="caseIds">An array of case IDs for the custom case selection.</param>
        /// <param name="customs">Custom fields are supported as well and must be submitted with their system name, prefixed with 'custom_', e.g. custom_comment</param>
        /// <returns>If successful, this method returns the new test plan entry including test runs.</returns>
        public RequestResult<PlanEntry> AddPlanEntry(ulong planId, ulong suiteId, string name = null, ulong? assignedToId = null,
            List<ulong> caseIds = null, JObject customs = null)
        {
            var uri = _CreateUri_(CommandType.Add, CommandAction.PlanEntry, planId);

            var planEntry = new PlanEntry
            {
                AssignedToId = assignedToId,
                SuiteId = suiteId,
                Name = name,
                CaseIds = caseIds
            };

            var jsonParams = JsonUtility.Merge(planEntry.GetJson(), customs);

            return SendCommand<PlanEntry>(uri, jsonParams);
        }

        /// <summary>Creates a new milestone.</summary>
        /// <param name="projectId">The ID of the project the milestone should be added to.</param>
        /// <param name="name">The name of the milestone (required).</param>
        /// <param name="description">The description of the milestone.</param>
        /// <param name="parentId">The ID of the parent milestone, if any (for sub-milestones) (available since TestRail 5.3).</param> 
        /// <param name="dueOn">The due date of the milestone (as UNIX timestamp).</param>
        /// <returns>If successful, this method returns the new milestone.</returns>
        public RequestResult<Milestone> AddMilestone(ulong projectId, string name, string description = null, ulong? parentId = null, DateTime? dueOn = null)
        {
            var uri = _CreateUri_(CommandType.Add, CommandAction.Milestone, projectId);

            var milestone = new Milestone
            {
                Name = name,
                Description = description,
                DueOn = dueOn,
                ParentId = parentId
            };

            return SendCommand<Milestone>(uri, milestone.GetJson());
        }
        #endregion Add Commands

        #region Update Commands
        /// <summary>updates an existing test case (partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="caseId">the ID of the test case</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeId">(optional)the ID of the case type</param>
        /// <param name="priorityId">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneId">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>if successful, this method returns the case id that was updated</returns>
        public CommandResult<ulong> UpdateCase(ulong caseId, string title, ulong? typeId = null,
            ulong? priorityId = null, string estimate = null, ulong? milestoneId = null, string refs = null, JObject customs = null)
        {
            // TODO: - At this time, this method only returns the id of the case that was updated.
            // We should return the case object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-cases#update_case

            return _UpdateCase_(caseId, title, typeId, priorityId, estimate, milestoneId, refs, customs);
        }

        /// <summary>updates an existing milestone (partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="milestoneId">id of the milestone</param>
        /// <param name="name">(optional)name of the milestone</param>
        /// <param name="description">(optional)description of the milestone</param>
        /// <param name="dueOn">(optional)date on which the milestone is due</param>
        /// <param name="isCompleted">true if a milestone is considered completed and false otherwise</param>
        /// <returns>if successful, this method returns the milestone id that was updated</returns>
        public CommandResult<ulong> UpdateMilestone(ulong milestoneId, string name = null,
            string description = null, DateTime? dueOn = null, bool? isCompleted = null)
        {
            // TODO: - At this time, this method only returns the id of the milestone that was updated.
            // We should return the milestone object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-milestones#update_milestone

            var uri = _CreateUri_(CommandType.Update, CommandAction.Milestone, milestoneId);

            var milestone = new Milestone
            {
                Name = name,
                Description = description,
                DueOn = dueOn,
                IsCompleted = isCompleted
            };

            return _SendCommand(uri, milestone.GetJson());
        }

        /// <summary>updates an existing test plan (partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="planId">id of the existing plan</param>
        /// <param name="name">(optional)name of the test plan </param>
        /// <param name="description">(optional)the description of the test plan</param>
        /// <param name="milestoneId">(optional)the id of the milestone to link to the test plan</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>if successful, this method returns the plan id that was updated</returns>
        public CommandResult<ulong> UpdatePlan(ulong planId, string name = null, string description = null, ulong? milestoneId = null, JObject customs = null)
        {
            // TODO: - At this time, this method only returns the id of the plan that was updated.
            // We should return the plan object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-plans#update_plan

            var uri = _CreateUri_(CommandType.Update, CommandAction.Plan, planId);

            var plan = new Plan
            {
                Name = name,
                Description = description,
                MilestoneId = milestoneId
            };

            var jsonParams = JsonUtility.Merge(plan.GetJson(), customs);

            return _SendCommand(uri, jsonParams);
        }

        /// <summary>updates one or more existing test runs in a plan (partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="planId">the ID of the plan the test run should be added to</param>
        /// <param name="entryId">the ID of the test plan entry</param>
        /// <param name="name">(optional)the name of the test run</param>
        /// <param name="assignedToId">(optional)the ID of the user the test run should be assigned to</param>
        /// <param name="caseIds">a list of case IDs for the custom case selection</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>if successful, this method returns the plan entry id that was updated</returns>
        public CommandResult<ulong> UpdatePlanEntry(ulong planId, string entryId, string name = null,
            ulong? assignedToId = null, List<ulong> caseIds = null, JObject customs = null)
        {
            // TODO: - At this time, this method only returns the id of the plan entry that was updated.
            // We should return the plan entry object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-plans#update_plan_entry

            var uri = _CreateUri_(CommandType.Update, CommandAction.PlanEntry, planId, null, null, entryId);

            var planEntry = new PlanEntry
            {
                AssignedToId = assignedToId,
                Name = name,
                CaseIds = caseIds
            };

            var jsonParams = JsonUtility.Merge(planEntry.GetJson(), customs);

            return _SendCommand(uri, jsonParams);
        }

        /// <summary>updates an existing project (admin status required; partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="projectId">the id of the existing project</param>
        /// <param name="projectName">the name of the project</param>
        /// <param name="announcement">(optional)the description of the project</param>
        /// <param name="showAnnouncement">(optional)true if the announcement should be displayed on
        /// the project's overview page and false otherwise</param>
        /// <param name="isCompleted">(optional)specifies whether a project is considered completed or not</param>
        /// <returns>if successful, this method returns the project id that was updated</returns>
        public CommandResult<ulong> UpdateProject(ulong projectId, string projectName, string announcement = null,
            bool? showAnnouncement = null, bool? isCompleted = null)
        {
            // TODO: - At this time, this method only returns the id of the project that was updated.
            // We should return the project object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-projects#update_project

            var uri = _CreateUri_(CommandType.Update, CommandAction.Project, projectId);

            var project = new Project
            {
                Name = projectName,
                Announcement = announcement,
                ShowAnnouncement = showAnnouncement,
                IsCompleted = isCompleted
            };

            return _SendCommand(uri, project.GetJson());
        }

        /// <summary>updates an existing test run (partial updates are supported, i.e. you can submit
        /// and update specific fields only)</summary>
        /// <param name="runId">the id of an existing run</param>
        /// <param name="name">(optional)name of the test run</param>
        /// <param name="description">(optional)description of the test run</param>
        /// <param name="milestoneId">(optional)the id of the milestone to link to the test run</param>
        /// <param name="caseIds">an array of case IDs for the custom case selection</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>if successful, this method return the run id that was updated</returns>
        public CommandResult<ulong> UpdateRun(ulong runId, string name = null, string description = null,
            ulong? milestoneId = null, HashSet<ulong> caseIds = null, JObject customs = null)
        {
            // TODO: - At this time, this method only returns the id of the run that was updated.
            // We should return the run object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-runs#update_run

            var includeAll = true;
            var existingRun = GetRun(runId);

            // validates whether we are in include all or custom case selection mode
            if (null != existingRun?.ProjectId && existingRun.SuiteId.HasValue && null != caseIds)
            {
                var atLeastOneCaseFoundInSuite = _CasesFoundInSuite(existingRun.ProjectId.Value, existingRun.SuiteId.Value, caseIds);

                if (atLeastOneCaseFoundInSuite)
                {
                    includeAll = false;
                }

                else
                {
                    return new CommandResult<ulong>(false, 0, new Exception("Case IDs not found in the Suite"));
                }
            }

            var uri = _CreateUri_(CommandType.Update, CommandAction.Run, runId);

            var newRun = new Run
            {
                Name = name,
                Description = description,
                MilestoneId = milestoneId,
                IncludeAll = includeAll,
                CaseIds = caseIds
            };

            var jsonParams = JsonUtility.Merge(newRun.GetJson(), customs);
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>updates an existing section (partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="sectionId">id of the section to update</param>
        /// <param name="name">name of the section</param>
        /// <param name="description">(optional)description of the section</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>if successful, this method returns the section id that was updated</returns>
        public CommandResult<ulong> UpdateSection(ulong sectionId, string name, string description = null, JObject customs = null)
        {
            // TODO: - At this time, this method only returns the id of the section that was updated.
            // We should return the section object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-sections#update_section

            if (string.IsNullOrWhiteSpace(name))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(name)));
            }

            var uri = _CreateUri_(CommandType.Update, CommandAction.Section, sectionId);

            var section = new Section
            {
                Id = sectionId,
                Name = name
            };

            var jsonParams = JsonUtility.Merge(section.GetJson(), customs);
            return _SendCommand(uri, jsonParams);
        }

        /// <summary>updates an existing test suite (partial updates are supported,
        /// i.e. you can submit and update specific fields only)</summary>
        /// <param name="suiteId">id of the suite to update</param>
        /// <param name="name">(optional)new name to update to</param>
        /// <param name="description">(optional)new description to update to</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>if successful, this method returns the test suite id that was updated</returns>
        public CommandResult<ulong> UpdateSuite(ulong suiteId, string name = null, string description = null, JObject customs = null)
        {
            // TODO: - At this time, the method only returns the id of the suite that was updated.
            // We should return the suite object instead as the official API documentation suggests.
            // http://docs.gurock.com/testrail-api2/reference-suites#update_suite

            var uri = _CreateUri_(CommandType.Update, CommandAction.Suite, suiteId);

            var s = new Suite
            {
                Name = name,
                Description = description
            };

            var jsonParams = JsonUtility.Merge(s.GetJson(), customs);

            return _SendCommand(uri, jsonParams);
        }
        #endregion Update Commands

        #region Close Commands
        /// <summary>closes a plan</summary>
        /// <param name="planId">id of the plan</param>
        /// <returns>true if successful</returns>
        public bool ClosePlan(ulong planId)
        {
            var uri = _CreateUri_(CommandType.Close, CommandAction.Plan, planId);
            var result = _CallPostEndpoint(uri);

            if (result.WasSuccessful)
            {
                var unused = JObject.Parse(result.Value);

                return result.WasSuccessful;
            }

            OnOperationFailed(this, $"Could not close plan : {result.Value}");

            return false;
        }

        /// <summary>closes a run</summary>
        /// <param name="runId">id of the run</param>
        /// <returns>true if successful</returns>
        public bool CloseRun(ulong runId)
        {
            var uri = _CreateUri_(CommandType.Close, CommandAction.Run, runId);
            var result = _CallPostEndpoint(uri);

            if (result.WasSuccessful)
            {
                var unused = JObject.Parse(result.Value);

                return result.WasSuccessful;
            }

            OnOperationFailed(this, $"Could not close run : {result.Value}");

            return false;
        }
        #endregion Close Commands

        #region Delete Commands
        /// <summary>Delete a milestone</summary>
        /// <param name="milestoneId">id of the milestone</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteMilestone(ulong milestoneId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Milestone, milestoneId);

            return _SendCommand(uri);
        }

        /// <summary>Delete a case</summary>
        /// <param name="caseId">id of the case to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteCase(ulong caseId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Case, caseId);

            return _SendCommand(uri);
        }

        /// <summary>Delete a plan</summary>
        /// <param name="planId">id of the plan to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeletePlan(ulong planId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Plan, planId);

            return _SendCommand(uri);
        }

        /// <summary>Delete a specific plan entry for a plan id</summary>
        /// <param name="planId">id of the plan</param>
        /// <param name="entryId">string representation of the GUID for the entryID</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeletePlanEntry(ulong planId, string entryId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.PlanEntry, planId, null, null, entryId);

            return _SendCommand(uri);
        }

        /// <summary>Delete the Project</summary>
        /// <param name="projectId">id of the project to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteProject(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Project, projectId);

            return _SendCommand(uri);
        }

        /// <summary>Delete the section</summary>
        /// <param name="sectionId">id of the section to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteSection(ulong sectionId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Section, sectionId);

            return _SendCommand(uri);
        }

        /// <summary>Delete the suite</summary>
        /// <param name="suiteId">id of the suite to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteSuite(ulong suiteId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Suite, suiteId);

            return _SendCommand(uri);
        }

        /// <summary>Delete the run</summary>
        /// <param name="runId">id of the run to delete</param>
        /// <returns>result of the deletion</returns>
        public CommandResult<ulong> DeleteRun(ulong runId)
        {
            var uri = _CreateUri_(CommandType.Delete, CommandAction.Run, runId);
            return _SendCommand(uri);
        }
        #endregion Delete Commands

        #region Get Commands
        /// <summary>gets a test</summary>
        /// <param name="testId">id of the test</param>
        /// <returns>information about the test</returns>
        public Test GetTest(ulong testId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Test, testId);

            return _GetItem_(CommandAction.Test, uri, Test.Parse);
        }

        /// <summary>gets tests associated with a run</summary>
        /// <param name="runId">id of the run</param>
        /// <returns>tests associated with the run</returns>
        public List<Test> GetTests(ulong runId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Tests, runId);

            return _GetItems_(CommandAction.Tests, uri, Test.Parse);
        }

        /// <summary>gets a case</summary>
        /// <param name="caseId">id of the case</param>
        /// <returns>information about the case</returns>
        public Case GetCase(ulong caseId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Case, caseId);

            return _GetItem_(CommandAction.Case, uri, Case.Parse);
        }

        /// <summary>gets cases associated with a suite</summary>
        /// <param name="projectId">id of the project</param>
        /// <param name="suiteId">id of the suite</param>
        /// <param name="sectionId">(optional) id of the section</param>
        /// <returns>cases associated with the suite</returns>
        public List<Case> GetCases(ulong projectId, ulong suiteId, ulong? sectionId = null)
        {
            var optionalSectionId = sectionId.HasValue ? $"&section_id={sectionId.Value}" : string.Empty;
            var options = $"&suite_id={suiteId}{optionalSectionId}";
            var uri = _CreateUri_(CommandType.Get, CommandAction.Cases, projectId, null, options);

            return _GetItems_(CommandAction.Cases, uri, Case.Parse);
        }

        /// <summary>returns a list of available test case custom fields</summary>
        /// <returns>a list of custom field definitions</returns>
        public List<CaseField> GetCaseFields()
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.CaseFields);

            return _GetItems_(CommandAction.CaseTypes, uri, CaseField.Parse);
        }

        /// <summary>returns a list of available case types</summary>
        /// <returns>a list of test case types, each has a unique ID and a name.</returns>
        public List<CaseType> GetCaseTypes()
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.CaseTypes);

            return _GetItems_(CommandAction.CaseTypes, uri, CaseType.Parse);
        }

        /// <summary>gets a suite</summary>
        /// <param name="suiteId">id of the suite</param>
        /// <returns>information about the suite</returns>
        public Suite GetSuite(ulong suiteId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Suite, suiteId);

            return _GetItem_(CommandAction.Suite, uri, Suite.Parse);
        }

        /// <summary>gets suites associated with a project</summary>
        /// <param name="projectId">id of the project</param>
        /// <returns>suites associated with the project</returns>
        public List<Suite> GetSuites(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Suites, projectId);

            return _GetItems_(CommandAction.Suites, uri, Suite.Parse);
        }

        /// <summary>gets a section</summary>
        /// <param name="sectionId">id of the section</param>
        /// <returns>information about the section</returns>
        public Section GetSection(ulong sectionId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Section, sectionId);

            return _GetItem_(CommandAction.Section, uri, Section.Parse);
        }

        /// <summary>gets sections associated with a suite</summary>
        /// <param name="projectId">id of the project</param>
        /// <param name="suiteId">id of the suite</param>
        /// <returns>sections associated with the suite</returns>
        public List<Section> GetSections(ulong projectId, ulong suiteId)
        {
            var options = $"&suite_id={suiteId}";
            var uri = _CreateUri_(CommandType.Get, CommandAction.Sections, projectId, null, options);

            return _GetItems_(CommandAction.Sections, uri, Section.Parse);
        }

        /// <summary>gets a run</summary>
        /// <param name="runId">id of the run</param>
        /// <returns>information about the run</returns>
        public Run GetRun(ulong runId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Run, runId);

            return _GetItem_(CommandAction.Run, uri, Run.Parse);
        }

        /// <summary>gets runs associated with a project</summary>
        /// <param name="projectId">id of the project</param>
        /// <returns>runs associated with the project</returns>
        public List<Run> GetRuns(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Runs, projectId);

            return _GetItems_(CommandAction.Runs, uri, Run.Parse);
        }

        /// <summary>gets a plan</summary>
        /// <param name="planId">id of the plan</param>
        /// <returns>information about the plan</returns>
        public Plan GetPlan(ulong planId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Plan, planId);

            return _GetItem_(CommandAction.Plan, uri, Plan.Parse);
        }

        /// <summary>gets plans associated with a project</summary>
        /// <param name="projectId">id of the project</param>
        /// <returns>plans associated with the project</returns>
        public List<Plan> GetPlans(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Plans, projectId);

            return _GetItems_(CommandAction.Plans, uri, Plan.Parse);
        }

        /// <summary>gets a milestone</summary>
        /// <param name="milestoneId">id of the milestone</param>
        /// <returns>information about the milestone</returns>
        public Milestone GetMilestone(ulong milestoneId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Milestone, milestoneId);

            return _GetItem_(CommandAction.Milestone, uri, Milestone.Parse);
        }

        /// <summary>gets milestones associated with a project</summary>
        /// <param name="projectId">id of the project</param>
        /// <returns>milestone associated with project</returns>
        public List<Milestone> GetMilestones(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Milestones, projectId);

            return _GetItems_(CommandAction.Milestones, uri, Milestone.Parse);
        }

        /// <summary>gets a project</summary>
        /// <param name="projectId">id of the project</param>
        /// <returns>information about the project</returns>
        public Project GetProject(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Project, projectId);

            return _GetItem_(CommandAction.Project, uri, Project.Parse);
        }

        /// <summary>gets all projects contained in the testrail instance</summary>
        /// <returns>list containing all the projects</returns>
        public List<Project> GetProjects()
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Projects);

            return _GetItems_(CommandAction.Projects, uri, Project.Parse);
        }

        /// <summary>Get User for user id</summary>
        /// <param name="userId">user id to search for</param>
        /// <returns>a User object</returns>
        public User GetUser(ulong userId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.User, userId);

            return _GetItem_(CommandAction.User, uri, User.Parse);
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

            var optionalParam = $"&email={email}";
            var uri = _CreateUri_(CommandType.Get, CommandAction.UserByEmail, null, null, optionalParam);

            return _GetItem_(CommandAction.UserByEmail, uri, User.Parse);
        }

        /// <summary>Get a list of users in the testrail instance</summary>
        /// <returns>List of users</returns>
        public List<User> GetUsers()
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Users);

            return _GetItems_(CommandAction.Users, uri, User.Parse);
        }

        /// <summary>Returns a list of test results for a test</summary>
        /// <param name="testId">id of the test</param>
        /// <param name="limit">(optional) maximum amount of test results to return, latest first</param>
        /// <returns>list containing the results for the given test</returns>
        public List<Result> GetResults(ulong testId, ulong? limit = null)
        {
            var optional = (limit.HasValue) ? $"&limit={limit.Value}" : string.Empty;
            var uri = _CreateUri_(CommandType.Get, CommandAction.Results, testId, null, optional);

            return _GetItems_(CommandAction.Results, uri, Result.Parse);
        }

        /// <summary>Return the list of test results for a test run and the case combination</summary>
        /// <param name="runId">id of the test run</param>
        /// <param name="caseId">id of the test case</param>
        /// <param name="limit">(optional) maximum amount of test results to return, latest first</param>
        /// <returns>list of test results for a case</returns>
        public List<Result> GetResultsForCase(ulong runId, ulong caseId, ulong? limit = null)
        {
            var optional = limit.HasValue ? $"&limit={limit.Value}" : string.Empty;
            var uri = _CreateUri_(CommandType.Get, CommandAction.ResultsForCase, runId, caseId, optional);

            return _GetItems_(CommandAction.ResultsForCase, uri, Result.Parse);
        }

        /// <summary>Return the list of test results for a test run</summary>
        /// <param name="runId">id of the rest run</param>
        /// <param name="limit">(optional) maximum amount of test results to return, latest first</param>
        /// <returns>list of test results for a test run</returns>
        public List<Result> GetResultsForRun(ulong runId, ulong? limit = null)
        {
            var optional = limit.HasValue ? $"&limit={limit.Value}" : string.Empty;
            var uri = _CreateUri_(CommandType.Get, CommandAction.ResultsForRun, runId, null, optional);

            return _GetItems_(CommandAction.ResultsForRun, uri, Result.Parse);
        }

        /// <summary>Returns the list of statuses available to test rail</summary>
        /// <returns>list of possible statuses</returns>
        public List<Status> GetStatuses()
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Statuses);

            return _GetItems_(CommandAction.Statuses, uri, Status.Parse);
        }

        /// <summary>Get a list of all available priorities</summary>
        /// <returns>list of priorities</returns>
        public List<Priority> GetPriorities()
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Priorities);

            return _GetItems_(CommandAction.Priorities, uri, Priority.Parse);
        }

        /// <summary>Returns a list of Config Groups available in a Project</summary>
        /// <param name="projectId">ID of the Project to return the Config Groups for</param>
        /// <returns>list of ConfigurationGroup</returns>
        public List<ConfigurationGroup> GetConfigurationGroups(ulong projectId)
        {
            var uri = _CreateUri_(CommandType.Get, CommandAction.Configs, projectId);

            return _GetItems_(CommandAction.Configs, uri, ConfigurationGroup.Parse);
        }
        #endregion Get Commands
        #endregion Public Methods

        #region Protected Methods
        /// <summary>executes a get request for an item</summary>
        /// <typeparam name="T">the type of item</typeparam>
        /// <param name="actionName">the name of item's node</param>
        /// <param name="uri">the uri for the request</param>
        /// <param name="parse">a method which parse json into the item</param>
        /// <returns>object of the supplied type containing information about the item</returns>
        protected T _GetItem_<T>(CommandAction actionName, string uri, Func<JObject, T> parse) where T : BaseTestRailType, new()
        {
            var result = _CallTestRailGetEndpoint(uri);

            if (!result.WasSuccessful)
            {
                OnOperationFailed(this, $"Could not get {actionName}: {result.Value}");

                return default(T);
            }

            var json = JObject.Parse(result.Value);

            return parse(json);
        }

        /// <summary>executes a get request for an item</summary>
        /// <typeparam name="T">the type of the item</typeparam>
        /// <param name="actionName">the name of the item's node</param>
        /// <param name="uri">the uri for the request</param>
        /// <param name="parse">a method which parses the json into the item</param>
        /// <returns>list of objects of the supplied type corresponding th supplied filters</returns>
        protected List<T> _GetItems_<T>(CommandAction actionName, string uri, Func<JObject, T> parse) where T : BaseTestRailType, new()
        {
            var items = new List<T>();
            var result = _CallTestRailGetEndpoint(uri);

            if (!result.WasSuccessful)
            {
                OnOperationFailed(this, $"Could not get {actionName}s: {result.Value}");
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
        /// <param name="commandType">the type of action the server is going to take (i.e. get, add, update, close)</param>
        /// <param name="actionName">the type of command the server is going to take (i.e. run, case, plan, etc)</param>
        /// <param name="id1">(optional)first id to include in the uri</param>
        /// <param name="id2">(optional)second id to include in the uri</param>
        /// <param name="options">(optional)additional options to include in the uri</param>
        /// <param name="id2Str">(optional)additional parameters to append to the uri</param>
        /// <returns>the newly created uri</returns>
        protected static string _CreateUri_(CommandType commandType, CommandAction actionName, ulong? id1 = null,
            ulong? id2 = null, string options = null, string id2Str = null)
        {
            var commandString = commandType.GetStringValue();
            var actionString = actionName.GetStringValue();

            var uri = $"?/api/v2/{commandString}_{actionString}{(id1.HasValue ? "/" + id1.Value : string.Empty)}{(id2.HasValue ? "/" + id2.Value : !string.IsNullOrWhiteSpace(id2Str) ? "/" + id2Str : string.Empty)}{(!string.IsNullOrWhiteSpace(options) ? options : string.Empty)}";

            return uri;
        }

        /// <summary>Add a case</summary>
        /// <param name="sectionId">section id to add the case to</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeId">(optional)the ID of the case type</param>
        /// <param name="priorityId">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneId">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customs">(optional)custom json params to add to the current json parameters</param>
        /// <returns>result of the command</returns>
        protected CommandResult<ulong> _AddCase_(ulong sectionId, string title, ulong? typeId = null,
            ulong? priorityId = null, string estimate = null, ulong? milestoneId = null, string refs = null, JObject customs = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(title)));
            }

            var uri = _CreateUri_(CommandType.Add, CommandAction.Case, sectionId);

            var tmpCase = new Case
            {
                Title = title,
                TypeId = typeId,
                PriorityId = priorityId,
                Estimate = estimate,
                MilestoneId = milestoneId,
                References = refs
            };

            var jsonParams = JsonUtility.Merge(tmpCase.GetJson(), customs);

            return _SendCommand(uri, jsonParams);
        }

        /// <summary>update an existing case</summary>
        /// <param name="caseId">the ID of the test case</param>
        /// <param name="title">title of the case</param>
        /// <param name="typeId">(optional)the ID of the case type</param>
        /// <param name="priorityId">(optional)the id of the case priority</param>
        /// <param name="estimate">(optional)the estimate, e.g. "30s" or "1m 45s"</param>
        /// <param name="milestoneId">(optional)the ID of the milestone to link to the test case</param>
        /// <param name="refs">(optional)a comma-separated list of references/requirements</param>
        /// <param name="customs">(optional)any extra parameters being passed into the update command</param>
        /// <returns>result of the command</returns>
        protected CommandResult<ulong> _UpdateCase_(ulong caseId, string title, ulong? typeId = null, ulong? priorityId = null, string estimate = null,
            ulong? milestoneId = null, string refs = null, JObject customs = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return new CommandResult<ulong>(false, 0, new ArgumentNullException(nameof(title)));
            }

            var uri = _CreateUri_(CommandType.Update, CommandAction.Case, caseId);

            var tmpCase = new Case
            {
                Title = title,
                TypeId = typeId,
                PriorityId = priorityId,
                Estimate = estimate,
                MilestoneId = milestoneId,
                References = refs
            };

            var jsonParams = JsonUtility.Merge(tmpCase.GetJson(), customs);

            return _SendCommand(uri, jsonParams);
        }
        #endregion Protected Methods

        #region Private Methods
        /// <summary>makes an http get call to the testrail</summary>
        /// <param name="uri">uri of the endpoint</param>
        /// <returns>result of the call</returns>
        private CommandResult _CallTestRailGetEndpoint(string uri)
        {
            uri = Url + uri;
            OnHttpRequestSent(this, new HttpRequestSentEventArgs("GET", new Uri(uri)));

            CommandResult commandResult;

            try
            {
                // Build request
                var request = new TestRailRequest(uri, "GET");

                request.AddHeaders(new Dictionary<string, string> { { "Authorization", AuthInfo } });
                request.Accepts("application/json");
                request.ContentType("application/json");

                // Send request
                commandResult = request.Execute();
            }

            catch (Exception e)
            {
                commandResult = new CommandResult(false, e.ToString());
            }

            if (!commandResult.WasSuccessful)
            {
                OnOperationFailed(this, $"HTTP RESPONSE: {commandResult.Value}");
            }

            else
            {
                OnHttpResponseReceived(this, commandResult.Value);
            }

            return commandResult;
        }

        /// <summary>makes an http post call to the testrail</summary>
        /// <param name="uri">uri of the endpoint</param>
        /// <param name="json">parameters to send formatted as a single json object</param>
        /// <returns>result of the call</returns>
        private CommandResult _CallPostEndpoint(string uri, JObject json = null)
        {
            uri = Url + uri;

            string postContent = null;

            if (null != json)
            {
                postContent = json.ToString();
            }

            OnHttpRequestSent(this, new HttpRequestSentEventArgs("POST", new Uri(uri), postContent));

            CommandResult commandResult;

            try
            {
                // Build request
                var request = new TestRailRequest(uri, "POST");

                request.AddHeaders(new Dictionary<string, string> { { "Authorization", AuthInfo } });
                request.Accepts("application/json");
                request.ContentType("application/json");

                // Add body
                if (!string.IsNullOrWhiteSpace(postContent))
                {
                    request.AddBody(postContent);
                }

                // Send request
                commandResult = request.Execute();
            }

            catch (Exception e)
            {
                commandResult = new CommandResult(false, e.ToString());
            }

            if (!commandResult.WasSuccessful)
            {
                OnOperationFailed(this, $"HTTP RESPONSE: {commandResult.Value}");
            }

            else
            {
                OnHttpResponseReceived(this, commandResult.Value);
            }

            return commandResult;
        }

        private RequestResult<T> SendCommand<T>(string uri, JObject jsonParams = null)
        {
            try
            {
                var result = _CallPostEndpoint(uri, jsonParams);

                return new RequestResult<T>(HttpStatusCode.OK, result.Value);
            }

            // If there is an error, will try to create a new result object
            // with the corresponding response code
            catch (Exception thrownException)
            {
                var message = thrownException.Message;

                // Return a response object for the most popular errors
                if (message.Contains("400"))
                    return new RequestResult<T>(HttpStatusCode.BadRequest, thrownException: thrownException);

                if (message.Contains("401"))
                    return new RequestResult<T>(HttpStatusCode.Unauthorized, thrownException: thrownException);

                if (message.Contains("403"))
                    return new RequestResult<T>(HttpStatusCode.Forbidden, thrownException: thrownException);

                if (message.Contains("404"))
                    return new RequestResult<T>(HttpStatusCode.NotFound, thrownException: thrownException);

                if (message.Contains("500"))
                    return new RequestResult<T>(HttpStatusCode.InternalServerError, thrownException: thrownException);

                if (message.Contains("502"))
                    return new RequestResult<T>(HttpStatusCode.BadGateway, thrownException: thrownException);

                if (message.Contains("503"))
                    return new RequestResult<T>(HttpStatusCode.ServiceUnavailable, thrownException: thrownException);

                if (message.Contains("504"))
                    return new RequestResult<T>(HttpStatusCode.GatewayTimeout, thrownException: thrownException);

                throw;
            }
        }

        /// <summary>Send a command to the server</summary>
        /// <param name="uri">uri to send</param>
        /// <param name="jsonParams">parameters to send formatted as a single json object</param>
        /// <returns>object containing if the command: was successful, the result value, and any exception that may have been thrown by the server</returns>
        private CommandResult<ulong> _SendCommand(string uri, JObject jsonParams = null)
        {
            Exception exception = null;
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

                            else if (JTokenType.String == token.Type) // for plan entry 
                            {
                                resultValue = (ulong)json["runs"][0]["id"];
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
                    exception = new Exception(result.Value);
                }
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }

            return new CommandResult<ulong>(wasSuccessful, resultValue, exception);
        }

        /// <summary>Determines if at least one of the case ids given is contained in the project and suite</summary>
        /// <param name="projectId">id of the project</param>
        /// <param name="suiteId">id of the suite</param>
        /// <param name="caseIds">collection of case ids to check</param>
        /// <returns>true if at least one case exists in the project and suite id combination, otherwise false</returns>
        private bool _CasesFoundInSuite(ulong projectId, ulong suiteId, ICollection<ulong> caseIds)
        {
            var validCases = GetCases(projectId, suiteId);

            return validCases.Any(tmpCase => tmpCase.Id.HasValue && caseIds.Contains(tmpCase.Id.Value));
        }

        /// <summary>Create a priority dictionary</summary>
        /// <returns>dictionary of priority ID (from test rail) to priority levels(where Higher value means higher priority)</returns>
        private Dictionary<ulong, int> _CreatePrioritiesDict()
        {
            var tmpDict = new Dictionary<ulong, int>();
            var priorityList = GetPriorities();

            foreach (var priority in priorityList.Where(priority => null != priority))
            {
                tmpDict[priority.Id] = priority.PriorityLevel;
            }

            return tmpDict;
        }
        #endregion Private Methods
    }
}
