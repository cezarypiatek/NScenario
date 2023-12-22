import React, {useEffect, useState} from 'react';

import './App.css';
//import  data from "./scenarios.json"
import DirectoryTree from 'antd/es/tree/DirectoryTree';
import { DataNode } from 'antd/es/tree';
import { Card, Col, Row, Space, Statistic, Timeline} from "antd";

import {
    CheckCircleOutlined,
    CloseCircleOutlined
} from '@ant-design/icons';



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
    if (!fileNode.children!.some(child => child.title === scenario.ScenarioTitle)) {
      fileNode.children!.push({
          title: scenario.ScenarioTitle,
          key: `${fileNode.key}/${scenario.ScenarioTitle}`,
          isLeaf: true,
          icon: scenario.Status === 0 ? (<CheckCircleOutlined style={{color:"green"}} />) :<CloseCircleOutlined style={{color:"red"}} />
      });
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



export function StatisticsCtr(props: {scenarios:Scenario[]}) {
  var success = props.scenarios.filter(value => value.Status === 0).length
  var failed = props.scenarios.filter(value => value.Status === 1).length
  return (
      <Row gutter={16} style={{width:"100%"}}>
        <Col span={8}>
          <Card bordered={false}>
            <Statistic
                title="Total"
                value={props.scenarios.length}
                precision={0}
                valueStyle={{ color: '#0000FF' }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card bordered={false}>
            <Statistic
                title="Success"
                value={success}
                precision={0}
                valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
        <Col span={8}>
          <Card bordered={false}>
            <Statistic
                title="Failed"
                value={failed}
                precision={0}
                valueStyle={{ color: '#cf1322' }}
            />
          </Card>
        </Col>
      </Row>
  );
}

export function StepCtr(props: {data:Step, prefix:string}) {
  return (
      <> <span style={{color:"blue"}}>Step {props.prefix}:</span> {`${props.data.Description}`}

          {props.data.Exception != null && (
              <pre style={{wordWrap: "break-word", whiteSpace: "pre-wrap", overflowX: "auto", padding:10, border: "1px solid red", borderRadius: 5}}><code>
              {props.data.Exception}
          </code></pre>
          )}

        {props.data.SubSteps && <Timeline style={{marginTop:"10px", paddingBottom:0}}
            mode={"left"}
            items={ props.data.SubSteps.map((value, index) => ({
                dot: value.Status === 3 ? (<CheckCircleOutlined style={{color:"green"}} />) :<CloseCircleOutlined style={{color:"red"}}/>,
                children: (<StepCtr data={value} prefix={`${props.prefix}.${index+1}`} />)}))
            }
        />}
      </>
  );
}
export function ScenarioTitleCtr(props: {data:Scenario}) {
    return (
        <div>
            <span style={{paddingRight: 10}}> {props.data.Status === 0 ? (<CheckCircleOutlined style={{color:"green"}} />) :<CloseCircleOutlined style={{color:"red"}} />}</span>
            <span>{  `SCENARIO: ${props.data.ScenarioTitle}`}</span>
        </div>
    );
}
const ScenarioCtr = React.memo((props: {data:Scenario, isSelected:boolean}) =>{
  return (
     <Card   id={props.data.ScenarioTitle.replace(/\W+/g,"-")} title={(<ScenarioTitleCtr data={props.data} /> )} style={{width:"auto", border: props.isSelected? "1px solid blue": "1px solid transparent",  transition: "border-color 0.5s ease" }}>
        <Timeline
style={{paddingBottom:0}}
        mode={"left"}
        items={ props.data.Steps.map((value, index) => ({
            dot: value.Status === 3 ? (<CheckCircleOutlined style={{color:"green"}} />) :<CloseCircleOutlined style={{color:"red"}} />,
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

function retriveData(): Scenario[]
{
    const dataStorage = document.getElementById('ScenarioData') as HTMLElement;
    if(dataStorage!=null && dataStorage.innerText.trim().length >0)
    {
        try {
            return JSON.parse(dataStorage.innerText) as Scenario[];
        }catch
        {
            return [];
        }
    }
    return [];
}

function App() {
    const scenarioData = retriveData();
    const treeData = generateTableOfContent(scenarioData);

    const [scenarioState, setScenarioState] = useState(scenarioData)
    const [treeState, setTreeState] = useState(treeData)
    const [selectedScenario, setSelectedScenario] = useState("")
    useEffect( () => {
        (async ()=>{
            if(scenarioData.length === 0)
            {
                const response = await fetch("/scenarios.json")
                const sampleData = await  response.json();
                setScenarioState(sampleData as Scenario[])
                setTreeState(generateTableOfContent(sampleData))
            }
        })()
    });

  return (
    <div className="App" style={{textAlign:"left", margin:0, padding:0}}>
        <Row style={{height: "calc(100vh - 10px)", background: "#EFEFEF"}}>
            <Col span={8}  style={{ height:"100%",  padding:"20px 0 20px 20px", overflowY:"scroll", overflowX:"hidden"}}>
                <DirectoryTree  multiple   defaultExpandAll={true} treeData={treeState} onSelect={(key, a)=>{
                    if(a.node.isLeaf && a.node.title != null) {

                        document.location = "#" + a.node.title.toString().replace(/\W+/g,"-")
                        setSelectedScenario(a.node.title.toString());
                    }
                }}
                />
            </Col>
            <Col span={16} style={{height: "100%"}}>
                <Row justify={"center"} style={{height: "150px", padding: "20px 20px 20px 10px"}}>
                    <StatisticsCtr scenarios={scenarioData}  />
                </Row>
                <Row style={{ height:"calc(100% - 200px)", padding:"0px 20px 20px 20px", overflowY: "scroll"}}>
                    <Space direction={"vertical"} size="middle" style={{ display: 'flex', width: "100%"}}>
                        {scenarioState.map(value => <ScenarioCtr data={value} isSelected={selectedScenario === value.ScenarioTitle} />)}
                    </Space>
                </Row>
                <Row align={"bottom"} style={{height:"50px", }}>
                    <Col span={24} style={{textAlign:"center", padding: "20px"}}>
                        <a href="https://github.com/cezarypiatek/NScenario" target="_blank" rel="noreferrer">NScenario</a> ©2023 Created by <a href="https://cezarypiatek.github.io/" target="_blank" rel="noreferrer">Cezary Piątek</a>
                    </Col>
                </Row>
            </Col>
        </Row>

    </div>
  );
}

export default App;
