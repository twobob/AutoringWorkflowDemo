# RequireAutoringConversionWorkflowDemo
 A repository to demonstrate how the require authoring conversion workflow works
 
 Contains 3 scenes :
 - ConversionWorkflowWithGeneratedAuthoring : Just adding component data one by one; having no clue of the underlying systems
- ConversionWorkflowWithCustomAuthoring : Having some sort of correspondence between the custom authoring component and the system. Also to show the issue with custom authoring component; that work with overlapping data component (Health in this case)
- ConversionWorkflowWithRequireAuthoring : Using simple monobehaviour in conjunction with the Require attribute to define behaviour based on the underlying systems.

# ConversionWorkflowWithGeneratedAuthoring
## Pros
- Great granularity
- Not duplicate components
## Cons
- Data component don't provide any hint to the underlying systems

# ConversionWorkflowWithCustomAuthoring
## Pros
- Custom authoring components give hint to the underlying systems and what can be expected in term of behaviour
## Cons
- Require lots of additional coding.
- Introduce the possibility to have duplicate data components.

# ConversionWorkflowWithRequireAuthoring
## Pros
- Custom authoring components give hint to the underlying systems and what can be expected in term of behaviour
- Very little coding required (just the system itself and a simple monobehaviour)
- The Require attribute take care of avoiding duplicate component data
## Cons
- Add additional components to the game object
- Can be hard to clean up gameobject as inspector don't remove unnecessary data component when removing the Custom authoring ones.
## Solution to Cons:
- Open the Windows > AuthoringComponentInspector : 
  - this will provide a list of all data component with editable properties (synch with the classic inspector)
  - The AuthoringComponentInspector has a button to synchronize the authoring components of the game object with this inspector. It take into account newly added data component, removing the non required data component from the game object itself and add missing one if and existing custom authoring component was updated.
  - The sync can be made automatic within the _Project Settings > Authoring Component Inspector Settings > Automatically sychronize data_ component
  - To clean the classic inspector, you can HideInInspector the generated data components using the toggle in _Project Settings > Authoring Component Inspector Settings > Hide/Show data component_ in the inspector
