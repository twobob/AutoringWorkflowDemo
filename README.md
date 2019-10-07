# RequireAutoringConversionWorkflowDemo
 A repository to demonstrate how the require authoring conversion workflow word
 
 Contains 3 scenes :
 - ConversionWorkflowWithGeneratedAuthoring : just adding component data one by one having no clue of the underlaying systems
- ConversionWorkflowWithCustomAuthoring : having some sort of correspondance between the custom authoring component and the system. Also to show the issue with custom authoring component taht work with overlaping data component (Health in this case)
- ConversionWorkflowWithRequireAuthoring : using simple monobehaviour in conjonction with the Require attribute to define behaviour based on the underlaying ssytems.

# ConversionWorkflowWithGeneratedAuthoring
## Pros
- great granualarity
- not duplicate components
## Cons
- data component don't provide any hint to the underlaying systems

# ConversionWorkflowWithCustomAuthoring
## Pros
- Custom authoring components give hint to the underlaying systems and what can be expected in term of behaviour
## Cons
- Requiere lots of additional coding.
- Introduce the posibility to have duplicate data components.

# ConversionWorkflowWithRequireAuthoring
## Pros
- Custom authoring components give hint to the underlaying systems and what can be expected in term of behaviour
- Very little coding requiered (just teh system itself and a simple monobehaviour)
- the Require attribute take care of avoiding duplicate component data
## Cons
- Add aditional components to the game object
- Can be hard to clean up gameobject as inspector don't remove unecessary data component when removing hte Custom authoring ones.
## Solution to Cons:
- Open the Windows > AuthoringComponentInspector : 
  - this will provide a list of all data component with editable properties (synch with the calsic inspector)
  - The AuthoringComponentInspector has a button to synchronize the authoring components of the game object with this inspector. It take into account newly added data component, removing the non required data component from the game object itself and add missing one if and exisitng custom authoring component was updated.
  - The sync can be made automatic, just edit the script Editor > AuthoringComponentInspector to set autoSync = true.
  - To clean the calssic inspector, you can HideInInspector the generated data components
