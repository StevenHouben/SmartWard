using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using NooSphere.Model.Primitives;
using SmartWard.Commands;
using SmartWard.Models;
using SmartWard.ViewModels;
using SmartWard.Infrastructure;



namespace SmartWard.Whiteboard.ViewModels
{
    public class AssignableClinicianViewModel : SmartWard.ViewModels.ClinicianViewModel
    {
        private WardNode _wardNode;
        private Patient _patient;
        private SmartWard.Models.Clinician.AssignmentType _assignmentType; 
        public AssignableClinicianViewModel(Clinician clinician, Patient patient, SmartWard.Models.Clinician.AssignmentType assignedShift, WardNode node)
            : base(clinician)
        {
            _patient = patient;
            _assignmentType = assignedShift;
            _wardNode = node;
        }

        private ICommand _toggleAssignmentCommand;

        public ICommand ToggleAssignmentCommand
        {
            get
            {
                return _toggleAssignmentCommand ?? (_toggleAssignmentCommand = new RelayCommand(
                    param => ToggleAssignment(param),
                    param => true
                    ));
            }
        }

        public void ToggleAssignment(object param)
        {
            AssignableClinicianViewModel cvm = param as AssignableClinicianViewModel;
            if (cvm == null) throw new ArgumentException("No valid parameter was given upon executing the command");

            //if Rounds
            if (_assignmentType == Models.Clinician.AssignmentType.Rounds)
            {
                //Get round activities
                var roundActivities = from round in _wardNode.ActivityCollection
                                      where round.Type == typeof(RoundActivity).Name
                                      select round as RoundActivity;
                //Find the ones relevant to the clinician in question and get an unfinished one if one exists
                var unfinished = roundActivities.FirstOrDefault(ra => ra.ClinicianId == cvm.Id && !ra.IsFinished);
                if (unfinished != null)
                {
                    //Find a visit for the relevant patient if one exists
                    var visit = unfinished.Visits.FirstOrDefault(v => v.PatientId == _patient.Id);
                    if (visit != null)
                    {
                        //Remove the visit and potentially the activity (Toggled off)
                        if (unfinished.Visits.Count > 1)
                        {
                            //More visits - remove this only
                            unfinished.removeVisit(visit);
                            _wardNode.UpdateActivity(unfinished);
                        }
                        else
                        {
                            //Only this visit - remove the entire activity
                            _wardNode.RemoveActivity(unfinished.Id);
                        }
                    }
                    else
                    {
                        //Add a visit (Toggled on)
                        unfinished.addVisit(new VisitActivity(_patient.Id));
                        _wardNode.UpdateActivity(unfinished);
                    }
                }
                else //None exist - create a new with one visit
                {
                    RoundActivity r = new RoundActivity(cvm.Id);
                    r.addVisit(new VisitActivity(_patient.Id));
                    _wardNode.AddActivity(r);
                }
                
            }
            else //Daily shifts
            {
                var assignment = cvm.Clinician.AssignedPatients.FirstOrDefault(a => a.Item1 == _patient.Id && a.Item2 == _assignmentType);
                if (assignment != null)
                {
                    //Assigned already contained, i.e toggled off - remove
                    cvm.Clinician.AssignedPatients.Remove(assignment);
                    _wardNode.UpdateUser(cvm.Clinician);
                }
                else
                {
                    cvm.Clinician.AssignedPatients.Add(new Tuple<string, SmartWard.Models.Clinician.AssignmentType>(_patient.Id, _assignmentType));
                    _wardNode.UpdateUser(cvm.Clinician);
                }
            }
        }
    }
}
