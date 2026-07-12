import React, {useContext, useEffect, useMemo, useState} from 'react';

import './App.css';
//import  data from "./scenarios.json"
import DirectoryTree from 'antd/es/tree/DirectoryTree';
import { DataNode } from 'antd/es/tree';
import { Card, Col, Empty, Input, Row, Statistic, Timeline} from "antd";

import {
    CheckCircleOutlined,
    ClockCircleOutlined,
    CloseCircleOutlined,
    SearchOutlined
} from '@ant-design/icons';
import {ICodeLocation, RepoPathResolver} from "./External";



interface Step  {
  Description: string;
  LineNumber: number;
  FilePath: string;
  ExecutionTime: string;
  Status: number;
  Exception: string | null;
  SubSteps?: Step[] | null;
}

interface Scenario  {
  ScenarioTitle: string;
  MethodName: string;
  FilePath: string;
  LineNumber: number;
  Status: number;
  Steps: Step[];
}






function getDirectoryPath(filePath:string) {
  if (!filePath) return '';

  // Replace backslashes with forward slashes for Windows paths
  const normalizedPath = filePath.replace(/\\/g, '/');

  // Find the last occurrence of a slash
  const lastSlashIndex = normalizedPath.lastIndexOf('/');

  // If there's no slash, return an empty string or handle as needed
  if (lastSlashIndex === -1) return '';

  // Return the substring from the start to the last slash
  return normalizedPath.substring(0, lastSlashIndex);
}

interface ScenarioTreeNode extends DataNode {
  scenarioTitle?: string;
}

function createDirectoryTree(scenarios: Scenario[], prefix: string): DataNode[] {
  const root: DataNode[] = [];
  scenarios.forEach(scenario => {
    const parts = scenario.FilePath.substring(prefix.length).split(/\\|\//).filter(item => item !== '');
    let currentLevel = root;

    // Iterate over parts except the last one (file name)
    for (let i = 0; i < parts.length - 1; i++) {
      const part = parts[i];
      let existingPath = currentLevel.find(p => p.title === part);

      if (!existingPath) {
        existingPath = { title: part, key: parts.slice(0, i + 1).join('/'), children: [] };
        currentLevel.push(existingPath);
      }

      currentLevel = existingPath.children!;
    }

    // Handle the file node
    const fileName = parts[parts.length - 1];
    let fileNode = currentLevel.find(node => node.title === fileName);
    if (!fileNode) {
      fileNode = { title: fileName, key: parts.join('/'), children: [] };
      currentLevel.push(fileNode);
    }

    // Add ScenarioTitle as a child of the file node, if it doesn't already exist
    if (!fileNode.children!.some(child => (child as ScenarioTreeNode).scenarioTitle === scenario.ScenarioTitle)) {
      fileNode.children!.push({
          title: <span className="scenario-tree-result"><span>{scenario.ScenarioTitle}</span><small>Method: {scenario.MethodName}</small></span>,
          key: `${fileNode.key}/${scenario.ScenarioTitle}`,
          scenarioTitle: scenario.ScenarioTitle,
          isLeaf: true,
          icon: scenario.Status === 0 ? <CheckCircleOutlined /> : <CloseCircleOutlined />
      } as ScenarioTreeNode);
    }
  });

  return root;
}

function findLongestCommonPrefix(strings: string[]): string {
  if (strings.length === 0) return "";

  let prefix = strings[0];
  for (let i = 1; i < strings.length; i++) {
    while (strings[i].indexOf(prefix) !== 0) {
      prefix = prefix.substring(0, prefix.length - 1);
      if (prefix === "") return "";
    }
  }
  return prefix;
}

enum TestResultType{
    All,
    Success,
    Failed
}

export function StatisticsCtr(props: {scenarios:Scenario[], type: TestResultType, onSetFilter: (type:TestResultType) => void }) {
  var success = props.scenarios.filter(value => value.Status === 0).length;
  var failed = props.scenarios.filter(value => value.Status === 1).length;
  const setFilter = (type:TestResultType) => {
    type = props.type === type && type !== TestResultType.All ? TestResultType.All : type;
    props.onSetFilter(type);
  };


  return (
      <Row gutter={8} style={{width:"100%"}} className="filter-row">
        <Col span={8}>
          <Card bordered={false} onClick={()=> setFilter(TestResultType.All)} className={props.type === TestResultType.All ? "selected-filter":""}>
            <Statistic
                title="Total"
                value={props.scenarios.length}
                precision={0}
                valueStyle={{ color: '#344054' }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card bordered={false} onClick={()=> setFilter(TestResultType.Success)} className={props.type === TestResultType.Success ? "selected-filter":""}>
            <Statistic
                title="Success"
                value={success}
                precision={0}
                valueStyle={{ color: '#15803d' }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card bordered={false} onClick={()=> setFilter(TestResultType.Failed)} className={props.type === TestResultType.Failed ? "selected-filter":""}>
            <Statistic
                title="Failed"
                value={failed}
                precision={0}
                valueStyle={{ color: '#b42318' }}
            />
          </Card>
        </Col>
      </Row>
  );
}

export function StepCtr(props: {data:Step, prefix:string}) {
  const globalServices = useContext(GlobalServicesContext);
  const scenarioFile = globalServices.pathResolver(props.data);
  const executionTime = formatExecutionTime(props.data.ExecutionTime);
  return (
      <div className="step-content">
        <div className="step-heading">
          <span><span className="step-number">Step {props.prefix}:</span> <a href={scenarioFile} target="_blank" rel="noreferrer">{props.data.Description}</a></span>
          {executionTime && <span className="execution-time" title={`Execution time: ${props.data.ExecutionTime}`}><ClockCircleOutlined /> {executionTime}</span>}
        </div>

          {props.data.Exception != null && (
              <pre className="exception-block"><code>
              {props.data.Exception}
          </code></pre>
          )}

        {props.data.SubSteps && props.data.SubSteps.length > 0 && <Timeline style={{marginTop:"10px", paddingBottom:0}}
            mode={"left"}
            items={ props.data.SubSteps.map((value, index) => ({
                dot: value.Status === 3 ? <CheckCircleOutlined /> : <CloseCircleOutlined />,
                children: (<StepCtr data={value} prefix={`${props.prefix}.${index+1}`} />)}))
            }
        />}
      </div>
  );
}

function formatExecutionTime(value: string): string {
  if (!value) return "";

  const parts = value.split(':');
  if (parts.length !== 3) return value;

  const totalSeconds = Number(parts[0]) * 3600 + Number(parts[1]) * 60 + Number(parts[2]);
  if (!Number.isFinite(totalSeconds)) return value;
  if (totalSeconds < 0.001) return `${Math.max(1, Math.round(totalSeconds * 1_000_000))} µs`;
  if (totalSeconds < 1) return `${Number((totalSeconds * 1000).toFixed(totalSeconds < 0.01 ? 2 : 1))} ms`;
  if (totalSeconds < 60) return `${Number(totalSeconds.toFixed(2))} s`;

  const minutes = Math.floor(totalSeconds / 60);
  return `${minutes}m ${Number((totalSeconds % 60).toFixed(1))}s`;
}
export function ScenarioTitleCtr(props: {data:Scenario}) {
    const globalServices = useContext(GlobalServicesContext);
    const scenarioFile = globalServices.pathResolver(props.data);
    return (
        <div className="scenario-heading">
            <span className="scenario-status">{props.data.Status === 0 ? <CheckCircleOutlined /> : <CloseCircleOutlined />}</span>
            <span className="scenario-heading-text">
                <span><span className="scenario-label">SCENARIO:</span> {props.data.ScenarioTitle}</span>
                <span className="scenario-method"><span>Method: </span><a rel="noreferrer" href={scenarioFile} target="_blank">{props.data.MethodName}</a></span>
            </span>
        </div>
    );
}
const ScenarioCtr = React.memo((props: {data:Scenario, isSelected:boolean}) =>{
  return (
     <Card className={`scenario-card scenario-${props.data.Status}${props.isSelected ? " scenario-selected" : ""}`} id={props.data.ScenarioTitle.replace(/\W+/g,"-")} title={(<ScenarioTitleCtr data={props.data} /> )}>
        <Timeline
style={{paddingBottom:0}}
        mode={"left"}
        items={ props.data.Steps.map((value, index) => ({
            dot: value.Status === 3 ? <CheckCircleOutlined /> : <CloseCircleOutlined />,
            children: (<StepCtr data={value} prefix={`${index+1}`} />)}))
        }
        />
     </Card>
  );
});

function generateTableOfContent(data:Scenario[])
{
    const dirPaths : string[] =  data.map(d => getDirectoryPath(d.FilePath))
    const rootDir = findLongestCommonPrefix(dirPaths);
    return createDirectoryTree(data, rootDir);
}

function retrieveData(): INScenarioData
{
    const dataStorage = document.getElementById('ScenarioData') as HTMLElement;
    if(dataStorage!=null && dataStorage.innerText.trim().length >0)
    {
        try {
            return JSON.parse(dataStorage.innerText) as INScenarioData;
        }catch
        {
        }
    }
    return {
        SourceControlInfo: {Revision: null, RepositoryUrl: null, RepositoryRootDir: null},
        Scenarios: []
    };
}

interface ISourceControlInfo
{
    RepositoryUrl:string | null,
    Revision:string | null,
    RepositoryRootDir:string | null
}

interface INScenarioData
{
    SourceControlInfo: ISourceControlInfo,
    Scenarios: Scenario[]
}

interface GlobalServices{
    pathResolver: ((location: ICodeLocation) => string)
}

const GlobalServicesContext = React.createContext<GlobalServices>({pathResolver: (location)=> location.FilePath })


function App() {
    const scenarioData = retrieveData();
    const [scenarioState, setScenarioState] = useState(scenarioData.Scenarios)
    const [selectedScenario, setSelectedScenario] = useState("")
    const [sourceControlState, setSourceControlState] = useState(scenarioData.SourceControlInfo)
    const [typeState, setTypeState] = useState(TestResultType.All)
    const [searchQuery, setSearchQuery] = useState("")
    const filteredScenarios = useMemo(() => {
        const normalizedQuery = searchQuery.trim().toLocaleLowerCase();
        return scenarioState.filter(scenario =>
            (typeState === TestResultType.All ||
            (typeState === TestResultType.Success && scenario.Status === 0) ||
            (typeState === TestResultType.Failed && scenario.Status === 1)) &&
            (!normalizedQuery || [scenario.ScenarioTitle, scenario.MethodName, scenario.FilePath]
                .some(value => value.toLocaleLowerCase().includes(normalizedQuery)))
        );
    }, [scenarioState, typeState, searchQuery]);
    const treeState = useMemo(() => generateTableOfContent(filteredScenarios), [filteredScenarios]);
    const filterLabel = typeState === TestResultType.Success ? "Successful" : typeState === TestResultType.Failed ? "Failed" : "All tests";
    let pathResolver = RepoPathResolver.TryToGetPathBuilder(sourceControlState, sourceControlState.RepositoryRootDir)

    useEffect( () => {
        (async ()=>{
            if(scenarioData.Scenarios.length === 0)
            {
                const response = await fetch("/scenarios.json")
                const sampleData : INScenarioData = await  response.json();
                setScenarioState(sampleData.Scenarios);
                setSourceControlState(sampleData.SourceControlInfo);
            }
        })()
    }, []);

  return (
    <div className={`App filter-${typeState}`} style={{textAlign:"left", margin:0, padding:0}}>
        <GlobalServicesContext.Provider value={{pathResolver: pathResolver}}>
        <Row className="report-shell">
            <Col span={8} className="explorer-panel">
                <div className="tree-heading">
                    <strong>Test explorer</strong>
                </div>
                <div className="global-filter-panel">
                    <StatisticsCtr scenarios={scenarioState} type={typeState} onSetFilter={type => { setTypeState(type); setSelectedScenario(""); }} />
                </div>
                <div className="scenario-search">
                    <Input
                        id="scenario-search-input"
                        aria-label="Search scenarios by title, method, or file name"
                        allowClear
                        prefix={<SearchOutlined />}
                        placeholder="Title, method, or file name…"
                        value={searchQuery}
                        onChange={event => { setSearchQuery(event.target.value); setSelectedScenario(""); }}
                    />
                    <span className="search-result-count">{filterLabel} · {filteredScenarios.length} shown{searchQuery.trim() ? " · search active" : ""}</span>
                </div>
                {treeState.length > 0 ? <DirectoryTree className="test-tree" multiple defaultExpandAll={true} treeData={treeState} onSelect={(key, a)=>{
                    const scenarioTitle = (a.node as ScenarioTreeNode).scenarioTitle;
                    if(a.node.isLeaf && scenarioTitle) {

                        document.location = "#" + scenarioTitle.replace(/\W+/g,"-")
                        setSelectedScenario(scenarioTitle);
                    }
                }}
                /> : <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} description={searchQuery.trim() ? "No scenarios match your search" : "No tests match this filter"} />}
            </Col>
            <Col span={16} className="details-panel">
                <Row className="report-list" style={{ height:"calc(100% - 50px)", padding:"20px", overflowY: "scroll"}}>
                    {filteredScenarios.length > 0
                        ? filteredScenarios.map(value => <ScenarioCtr key={`${value.FilePath}-${value.MethodName}`} data={value} isSelected={selectedScenario === value.ScenarioTitle} />)
                        : <Empty className="report-empty" description={searchQuery.trim() ? "No scenarios match your search" : "No tests match this filter"} />}
                </Row>
                <Row align={"bottom"} style={{height:"50px", }}>
                    <Col span={24} style={{textAlign:"center", padding: "20px"}}>
                        <a href="https://github.com/cezarypiatek/NScenario" target="_blank" rel="noreferrer">NScenario</a> ©2026 Created by <a href="https://cezarypiatek.github.io/" target="_blank" rel="noreferrer">Cezary Piątek</a>
                    </Col>
                </Row>
            </Col>
        </Row>
        </GlobalServicesContext.Provider>
    </div>
  );
}

export default App;
